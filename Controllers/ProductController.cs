using COSMESTIC.Models.Data;
using COSMESTIC.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

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
