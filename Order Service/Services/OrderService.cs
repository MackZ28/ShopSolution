using Common.OrderData;
using Common.OrderData.DTOs;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.DTOs;
using OrderService.Interfaces;

namespace OrderService.Services
{
    internal class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IKafkaProducerService _kafkaProducer;
        private readonly IAuthApiService _authApiService; 
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            AppDbContext context,
            IKafkaProducerService kafkaProducer,
            IAuthApiService authApiService, 
            ILogger<OrderService> logger)
        {
            _context = context;
            _kafkaProducer = kafkaProducer;
            _authApiService = authApiService; 
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(CreateOrderDTO dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    ProductId = dto.ProductId,
                    ProductName = dto.ProductName,
                    Quantity = dto.Quantity,
                    UserId = dto.UserId,
                    CreatedAt = DateTime.UtcNow
                    //PaymentPassed = false // если нужно
                };

                var res = await _context.Orders.AddAsync(order);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // 2. Отправляем событие в Kafka
                await _kafkaProducer.ProduceAsync("order-created", order.Id.ToString(), new
                {
                    order.Id,
                    order.ProductName,
                    order.Quantity,
                    order.CreatedAt
                });

                return order;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<OrderWithUserDto>> GetOrdersWithUserInfoAsync()
        {
            var orders = await _context.Orders.ToListAsync();
            var userIds = orders.Select(o => o.UserId).Distinct().ToList();

            // Получаем информацию о пользователях одним запросом
            var users = await _authApiService.GetUsersByIdsAsync(userIds);
            var userDict = users.ToDictionary(u => u.Id);

            var result = orders.Select(order => new OrderWithUserDto
            {
                Id = order.Id,
                ProductId = order.ProductId,
                ProductName = order.ProductName,
                Quantity = order.Quantity,
                CreatedAt = order.CreatedAt,
                User = userDict.TryGetValue(order.UserId, out var user) ? user : null
            }).ToList();

            return result;
        }
        public Task<bool> DeleteOrderAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Order?> GetOrderByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrderAsync(Guid id, UpdateOrderDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
