namespace RSM.Application.Dtos
{
    public class OrderListDto
    {
        public int OrderId {get; set;}
        public string CustomerName {get; set;} = string.Empty;
        public DateTime OrderDate {get; set;}
        public string Country {get; set;} = string.Empty;
        public string Status {get; set;} = string.Empty;

        public decimal TotalAmount {get; set;}
        public int ProductCount {get; set;}
    }
}