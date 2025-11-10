namespace OrderService.Services.Infrastructure
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string DefaultTopic { get; set; } = string.Empty;
    }
}
