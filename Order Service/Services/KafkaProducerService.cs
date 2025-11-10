using Confluent.Kafka;
using Microsoft.Extensions.Options;
using OrderService.Interfaces;
using OrderService.Services.Infrastructure;
using System.Text.Json;

namespace OrderService.Services
{
    public class KafkaProducerService : IKafkaProducerService, IAsyncDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducerService> _logger;
        private readonly KafkaSettings _settings;

        public KafkaProducerService(IOptions<KafkaSettings> settings, 
                                    ILogger<KafkaProducerService> logger)
        {
            _settings = settings.Value;
            _logger = logger;

            var config = new ProducerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                Acks = Acks.All, // гарантирует доставку
                EnableIdempotence = true, // защита от дубликатов
                LingerMs = 5, // оптимизация пакетов
                BatchSize = 32 * 1024, // 32 KB
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 100
            };

            _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) =>
            {
                _logger.LogError("Kafka Producer error: {Reason}", e.Reason);
            })
            .Build();
        }


        public async Task ProduceAsync<T>(string topic, string key, T message, CancellationToken cancellationToken = default)
        {
            try
            {
                var value = JsonSerializer.Serialize(message);

                var dr = await _producer.ProduceAsync(topic, new Message<string, string>
                {
                    Key = key,
                    Value = value
                }, cancellationToken);

                _logger.LogInformation(
                    "Delivered message to {TopicPartitionOffset} | Key={Key}",
                    dr.TopicPartitionOffset, key);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "Delivery failed: {Reason}", ex.Error.Reason);
                throw; // лучше пробросить, чтобы сервис знал об ошибке
            }
        }

        public ValueTask DisposeAsync()
        {
            _logger.LogInformation("Closing Kafka producer...");
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
