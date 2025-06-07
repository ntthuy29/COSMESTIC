using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;

namespace COSMESTIC.Models
{
    public class OverView
    {
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }

        // Doanh thu theo danh mục
        public List<RevenueByCatalog> RevenueByCatalogList { get; set; }
        public TopProduct topProduct { get; set; }
    }

    public class RevenueByCatalog
    {
        public string CatalogName { get; set; }
        public decimal Revenue { get; set; }
    }
    public class TopProduct
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal TotalRevenue { get; set; }
    }

}
