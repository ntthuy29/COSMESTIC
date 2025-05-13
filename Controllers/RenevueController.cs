using COSMESTIC.Models.Data;
using Microsoft.AspNetCore.Mvc;

namespace COSMESTIC.Controllers
{
    public class RenevueController : Controller
    {
        private readonly AppDbContext _context;

        public RenevueController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Renevue()
        {
            var totalRevenue = _context.orderDetails.Sum(od => od.quantity * od.unitPrice);
            var totalOrders = _context.Orders.Count();
            var totalCustomers = _context.Users.Count();

            var topProducts = _context.orderDetails
                .GroupBy(od => od.productID)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(x => x.quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToList();

           

            return View();
        }
    }
}
