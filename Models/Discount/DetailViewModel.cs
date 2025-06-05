namespace COSMESTIC.Models.Discount
{
    public class DetailViewModel
    {
        public int discountID { get; set; }
        public string name { get; set; } // Tên mã (tương ứng discountType)
        public decimal value { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool isActive { get; set; }
        public decimal discountAmount { get; set; }

        public const int  maxUsageLimit = 50;
        public int usageCount { get; set; }
        public string status { get; set; }
    }
}
