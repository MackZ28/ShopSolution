namespace NotificationService.Infrastructure
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public List<string> Topics { get; set; } = new();
        public string AutoOffsetReset { get; set; } = "earliest";
    }
}


