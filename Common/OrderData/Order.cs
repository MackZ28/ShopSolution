namespace Common.OrderData
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        //public bool PaymentPassed { get; set; }
    }
}
