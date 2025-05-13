namespace COSMESTIC.Models.Revenue
{
    public class RevenueItem
    {
        public string Key { get; set; } // Danh mục, trạng thái, hoặc "Tổng"
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
    }
}
