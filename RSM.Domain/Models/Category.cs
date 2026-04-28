namespace RSM.Domain.Models
{
    public class Category
    {
        public required int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}