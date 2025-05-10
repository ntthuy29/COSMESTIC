namespace COSMESTIC.Models.Order
{
    public class ListOrderModel
    {
        public int OrderID { get; set; }
        public int? UserID { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
        public int? DeliveryID { get; set; }
    }
}
 