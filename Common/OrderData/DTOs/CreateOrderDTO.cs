using System.ComponentModel.DataAnnotations;

namespace Common.OrderData.DTOs
{
    public class CreateOrderDTO
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [StringLength(100)]
        public string ProductName { get; set; }
        
        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}
