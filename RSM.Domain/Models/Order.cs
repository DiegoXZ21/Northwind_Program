namespace RSM.Domain.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public required string CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        
        public required string ShipCity { get; set; }
    }
}