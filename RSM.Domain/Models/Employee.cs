namespace RSM.Domain.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Title { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        //Relationships with other entities
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}