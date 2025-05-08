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
        public async Task<IActionResult> Product()
        {

            var products = await _context.Products.ToListAsync(); // Lấy tất cả sản phẩm từ DB

            var catalogs = await _context.Catalogs.ToListAsync();
            ViewBag.Catalogs = catalogs;

            return View(products); // Truyền danh sách qua View
        }
        public IActionResult ProductDetail(int id, string name)
        {
            var product = _context.Products.Find( id);
            if(product == null)
            {
                return RedirectToAction("Product", "Product");
            }
            else
            {
                return View(product);
            }
        }
        
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return PartialView("_ProductListPartial", new List<Products>());
            }

            // Truy vấn sản phẩm có tên chứa query
            var products = await _context.Products
                .Where(p => p.productName.Contains(query))
                .ToListAsync();

            // Kiểm tra xem có sản phẩm hay không
            if (products == null || !products.Any())
            {
                return PartialView("_ProductListPartial", new List<Products>());
            }

            // Trả về kết quả tìm kiếm
            return PartialView("_ProductListPartial", products);
        }
        [HttpGet]
        [Route("Product/GetByCatalog/{catalogId}")]
        public async Task<IActionResult> GetByCatalog(int catalogId)
        {
            Console.WriteLine(catalogId);  // Kiểm tra catalogId
            var products = await _context.Products
                .Where(p => p.catalogID == catalogId)
                .ToListAsync();

            foreach (var p in products)
            {
                Console.WriteLine(p.catalogID);
            }

            return PartialView("Catalog", products);
        }



    }
}
