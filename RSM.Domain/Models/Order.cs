using System.ComponentModel.DataAnnotations.Schema;

namespace RSM.Domain.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        [Column("ShipVia")]
        public int? ShipperId { get; set; }
        public decimal? Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }

        public Customer Customer { get; set; } = null!;
        public Shipper Shipper { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
    }

}