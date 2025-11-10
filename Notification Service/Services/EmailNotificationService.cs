using NotificationService.Models;

namespace NotificationService.Services
{
    public class EmailNotificationService : INotificationService
    {
        private readonly ILogger<EmailNotificationService> _logger;

        public EmailNotificationService(ILogger<EmailNotificationService> logger)
        {
            _logger = logger;
        }

        public async Task SendOrderNotificationAsync(OrderCreatedEvent orderEvent, CancellationToken cancellationToken = default)
        {
            // Simulate sending notification
            _logger.LogInformation("📧 Sending notification for Order #{OrderId}", orderEvent.Id);
            
            // TODO: Implement actual notification logic
            // Examples:
            // - await SendEmailAsync(orderEvent);
            // - await SendSmsAsync(orderEvent);
            // - await SendPushNotificationAsync(orderEvent);
            
            // Simulate async operation
            await Task.Delay(100, cancellationToken);
            
            _logger.LogInformation(
                "✅ Notification sent successfully: Order #{OrderId} - {ProductName} x{Quantity} created at {CreatedAt}",
                orderEvent.Id,
                orderEvent.ProductName,
                orderEvent.Quantity,
                orderEvent.CreatedAt);
        }
    }
}

