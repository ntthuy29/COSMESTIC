namespace COSMESTIC.Models.Discount
{
    public class ListDiscountModel
    {
        public int discountID { get; set; }
        public string name { get; set; }
        public decimal value { get; set; }

        public string startDate { get; set; }
        public string endDate { get; set; }

        public bool  isActive { get; set; }
        // Navigation property
    }
}
