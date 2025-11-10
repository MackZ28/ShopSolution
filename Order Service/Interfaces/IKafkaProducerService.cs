namespace OrderService.Interfaces
{
    internal interface IKafkaProducerService
    {
        Task ProduceAsync<T>(string topic, string key, T message, CancellationToken cancellationToken = default);
    }
}
