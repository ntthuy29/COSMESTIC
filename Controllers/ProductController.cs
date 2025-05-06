using COSMESTIC.Models.Data;
using Microsoft.AspNetCore.Mvc;


namespace COSMESTIC.Controllers
{
    public class ProductController: Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Product()
        {
            var products = _context.Products.ToList(); // Lấy tất cả sản phẩm từ DB
            return View(products); // Truyền danh sách qua View
        }
    }
}
