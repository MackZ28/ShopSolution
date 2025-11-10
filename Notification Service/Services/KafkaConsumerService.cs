using Confluent.Kafka;
using Microsoft.Extensions.Options;
using NotificationService.Infrastructure;
using NotificationService.Models;
using System.Text.Json;

namespace NotificationService.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly KafkaSettings _settings;
        private readonly IServiceProvider _serviceProvider;

        public KafkaConsumerService(
            IOptions<KafkaSettings> settings,
            ILogger<KafkaConsumerService> logger,
            IServiceProvider serviceProvider)
        {
            _settings = settings.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;

            var config = new ConsumerConfig
            {
                BootstrapServers = _settings.BootstrapServers,
                GroupId = _settings.GroupId,
                AutoOffsetReset = _settings.AutoOffsetReset == "earliest" 
                    ? AutoOffsetReset.Earliest 
                    : AutoOffsetReset.Latest,
                EnableAutoCommit = false,
                EnableAutoOffsetStore = false
            };

            _consumer = new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, e) =>
                {
                    _logger.LogError("Kafka Consumer error: {Reason}", e.Reason);
                })
                .Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Kafka Consumer Service is starting.");

            try
            {
                _consumer.Subscribe(_settings.Topics);
                _logger.LogInformation("Subscribed to topics: {Topics}", string.Join(", ", _settings.Topics));

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));

                        if (consumeResult != null)
                        {
                            _logger.LogInformation(
                                "Received message: Topic={Topic}, Partition={Partition}, Offset={Offset}, Key={Key}",
                                consumeResult.Topic,
                                consumeResult.Partition.Value,
                                consumeResult.Offset.Value,
                                consumeResult.Message.Key);

                            await ProcessMessageAsync(consumeResult.Message, stoppingToken);

                            // Commit offset after successful processing
                            _consumer.Commit(consumeResult);
                            _consumer.StoreOffset(consumeResult);
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error consuming message: {Reason}", ex.Error.Reason);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unexpected error while processing message");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka Consumer Service is stopping.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatal error in Kafka Consumer Service");
            }
            finally
            {
                _consumer.Close();
                _consumer.Dispose();
            }
        }

        private async Task ProcessMessageAsync(Message<string, string> message, CancellationToken cancellationToken)
        {
            try
            {
                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message.Value);

                if (orderEvent == null)
                {
                    _logger.LogWarning("Failed to deserialize message: {Value}", message.Value);
                    return;
                }

                _logger.LogInformation(
                    "Processing order: OrderId={OrderId}, Product={ProductName}, Quantity={Quantity}, CreatedAt={CreatedAt}",
                    orderEvent.Id,
                    orderEvent.ProductName,
                    orderEvent.Quantity,
                    orderEvent.CreatedAt);

                // TODO: Add notification logic here
                // For example:
                // - Send email
                // - Send SMS
                // - Push notification
                // - Log to database

                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetService<INotificationService>();
                
                if (notificationService != null)
                {
                    await notificationService.SendOrderNotificationAsync(orderEvent, cancellationToken);
                }
                else
                {
                    _logger.LogInformation(
                        "✅ Order notification would be sent for Order #{OrderId}: {ProductName} x{Quantity}",
                        orderEvent.Id,
                        orderEvent.ProductName,
                        orderEvent.Quantity);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing message: {Value}", message.Value);
            }
        }

        public override void Dispose()
        {
            _consumer?.Dispose();
            base.Dispose();
        }
    }
}


