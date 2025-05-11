namespace COSMESTIC.Models.Revenue
{
    public class RevenueReportViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Thống kê theo danh mục
        public List<RevenueItem> CategoryStats { get; set; }
        public List<string> CategoryLabels { get; set; }
        public List<decimal> CategoryRevenueData { get; set; }
        public List<int> CategoryOrderCountData { get; set; }

        // Thống kê theo trạng thái
        public List<RevenueItem> StatusStats { get; set; }
        public List<string> StatusLabels { get; set; }
        public List<decimal> StatusRevenueData { get; set; }
        public List<int> StatusOrderCountData { get; set; }

        // Thống kê toàn hệ thống
        public List<RevenueItem> TotalStats { get; set; }
        public List<string> TotalLabels { get; set; }
        public List<decimal> TotalRevenueData { get; set; }
        public List<int> TotalOrderCountData { get; set; }
    }
}
