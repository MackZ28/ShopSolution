using NotificationService.Models;

namespace NotificationService.Services
{
    public interface INotificationService
    {
        Task SendOrderNotificationAsync(OrderCreatedEvent orderEvent, CancellationToken cancellationToken = default);
    }
}


