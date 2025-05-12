using COSMESTIC.Models.Data;
using COSMESTIC.Models.Revenue;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace COSMESTIC.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "admin")]
        public IActionResult Home()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        public IActionResult Product()
        {
            return RedirectToAction("Index", "AdminProduct");
        }
        [Authorize(Roles = "admin")]
        public IActionResult Discount()
        {
            return RedirectToAction("Index", "Discount");
        }
        [Authorize(Roles = "admin")]
        public IActionResult User()
        {
            return RedirectToAction("Index", "Account");
        }
        [Authorize(Roles = "admin")]
        public IActionResult Order()
        {
            return RedirectToAction("IndexAdminOrder", "Order");
        }
        [Authorize(Roles = "admin")]
        public IActionResult Catalog()
        {
            return RedirectToAction("Index", "Catalog");
        }
        [Authorize(Roles = "admin")]
        public IActionResult Statistics()
        {
            return RedirectToAction("Index", "Statistics");
        }
        [Authorize(Roles = "admin")]
        public IActionResult Revenue()
        {
            var model = new RevenueReportViewModel
            {
                StartDate = DateTime.Now.AddMonths(-1),
                EndDate = DateTime.Now,
                CategoryStats = new List<RevenueItem>(),
                StatusStats = new List<RevenueItem>(),
                TotalStats = new List<RevenueItem>(),
                CategoryLabels = new List<string>(),
                CategoryRevenueData = new List<decimal>(),
                CategoryOrderCountData = new List<int>(),
                StatusLabels = new List<string>(),
                StatusRevenueData = new List<decimal>(),
                StatusOrderCountData = new List<int>(),
                TotalLabels = new List<string>(),
                TotalRevenueData = new List<decimal>(),
                TotalOrderCountData = new List<int>()
            };
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Revenue(DateTime? startDate, DateTime? endDate)
        {
            // Mặc định khoảng thời gian nếu không có giá trị
            startDate ??= DateTime.Now.AddMonths(-1);
            endDate ??= DateTime.Now;

            var ordersQuery = _context.Orders
                .Include(o => o.orderDetails)
                .ThenInclude(od => od.products)
                .ThenInclude(p => p.catalog)
                .Where(o => o.orderDate >= startDate && o.orderDate <= endDate);

            var model = new RevenueReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                CategoryStats = new List<RevenueItem>(),
                StatusStats = new List<RevenueItem>(),
                TotalStats = new List<RevenueItem>(),
                CategoryLabels = new List<string>(),
                CategoryRevenueData = new List<decimal>(),
                CategoryOrderCountData = new List<int>(),
                StatusLabels = new List<string>(),
                StatusRevenueData = new List<decimal>(),
                StatusOrderCountData = new List<int>(),
                TotalLabels = new List<string>(),
                TotalRevenueData = new List<decimal>(),
                TotalOrderCountData = new List<int>()
            };

            // 1. Thống kê theo danh mục
            var revenueByCategory = await ordersQuery
                .SelectMany(o => o.orderDetails)
                .GroupBy(od => od.products.catalog.catalogName)
                .Select(g => new RevenueItem
                {
                    Key = g.Key,
                    TotalRevenue = g.Sum(od => od.quantity * od.unitPrice),
                    OrderCount = g.Select(od => od.orderID).Distinct().Count()
                })
                .ToListAsync();

            model.CategoryStats = revenueByCategory;
            model.CategoryLabels = revenueByCategory.Select(r => r.Key).ToList();
            model.CategoryRevenueData = revenueByCategory.Select(r => r.TotalRevenue).ToList();
            model.CategoryOrderCountData = revenueByCategory.Select(r => r.OrderCount).ToList();

            // 2. Thống kê theo trạng thái
            var revenueByStatus = await ordersQuery
                .GroupBy(o => o.status)
                .Select(g => new RevenueItem
                {
                    Key = g.Key,
                    TotalRevenue = g.Sum(o => o.totalAmount),
                    OrderCount = g.Count()
                })
                .ToListAsync();

            model.StatusStats = revenueByStatus;
            model.StatusLabels = revenueByStatus.Select(r => r.Key).ToList();
            model.StatusRevenueData = revenueByStatus.Select(r => r.TotalRevenue).ToList();
            model.StatusOrderCountData = revenueByStatus.Select(r => r.OrderCount).ToList();

            // 3. Thống kê toàn hệ thống
            var totalRevenue = await ordersQuery.SumAsync(o => o.totalAmount);
            var orderCount = await ordersQuery.CountAsync();

            model.TotalStats.Add(new RevenueItem
            {
                Key = "Tổng",
                TotalRevenue = totalRevenue,
                OrderCount = orderCount
            });
            model.TotalLabels = new List<string> { "Tổng" };
            model.TotalRevenueData = new List<decimal> { totalRevenue };
            model.TotalOrderCountData = new List<int> { orderCount };

            return View(model);
        }
    }
}
