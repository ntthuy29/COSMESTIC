using COSMESTIC.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace COSMESTIC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Customer()
        {
            var products = _context.Products.ToList(); // Lấy tất cả sản phẩm từ DB
            return View(products); // Truyền danh sách qua View
        }
    }
}
