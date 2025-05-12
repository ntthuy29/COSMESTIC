namespace COSMESTIC.Models.Order
{
    public class OrderViewModel
    {
        public string OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int? TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}
