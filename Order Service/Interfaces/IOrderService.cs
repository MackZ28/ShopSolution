using Common.OrderData;
using Common.OrderData.DTOs;
using OrderService.DTOs;

namespace OrderService.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CreateOrderDTO dto);
        Task<Order?> GetOrderByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<bool> UpdateOrderAsync(Guid id, UpdateOrderDTO dto);
        Task<bool> DeleteOrderAsync(Guid id);
        Task<List<OrderWithUserDto>> GetOrdersWithUserInfoAsync();
    }
}
