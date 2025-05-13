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
        public IActionResult ProductDetail(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var product = _context.Products.Include(p => p.ProductReviews)
                                           .ThenInclude(r => r.user)
                                           .FirstOrDefault(p => p.productID == id);
            if(product == null)
            {
                return RedirectToAction("Product", "Product");
            }
            bool hasPurchased = false;
            bool hasReviewed = false;
            if (userId != null)
            {
                var userOrders = _context.Orders
                    .Where(o => o.userID == userId && o.status == "Shipped")
                    .Include(o => o.orderDetails)
                    .ToList();
                hasPurchased = _context.Orders
                                       .Where(o => o.userID == userId && o.status == "Shipped")
                                       .Any(o => o.orderDetails.Any(od => od.productID == id));
                // Nếu có bất kỳ đơn hàng nào chưa được đánh giá thì hasReviewed sẽ là true
                hasReviewed = userOrders.Any(o => o.orderDetails != null && o.orderDetails.Any(od => od.productID == id) &&
                                       !_context.ProductReView
                                           .Any(r => r.userID == userId && r.productID == id && r.orderID == o.orderID));

            }
            ViewBag.HasPurchased = hasPurchased;
            ViewBag.HasReviewed = hasReviewed;
            return View(product);
        }
        [HttpPost]
        public IActionResult AddReview(int productID, int rating, string comment)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var hasPurchased = _context.Orders
                .Where(o => o.userID == userId && o.status == "Shipped")
                .Any(o => o.orderDetails.Any(od => od.productID == productID));
            if (!hasPurchased)
            {
                TempData["ErrorMessage"] = "Bạn phải mua sản phẩm trước khi đánh giá.";
                return RedirectToAction("ProductDetail", new { productID = productID });
            }
            var lastOrder = _context.Orders
                .Where(o => o.userID == userId && o.status == "Shipped")
                .OrderByDescending(o => o.orderDate)
                .FirstOrDefault();
            if (lastOrder == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có đơn hàng đã giao để đánh giá.";
                return RedirectToAction("ProductDetail", new { productID = productID });
            }
            var hasReviewed = _context.ProductReView
                .Any(r => r.userID == userId && r.productID == productID && r.orderID == lastOrder.orderID);
            if (hasReviewed)
            {
                TempData["ErrorMessage"] = "Bạn đã đánh giá sản phẩm này rồi. Bạn có thể đánh giá lại ở lần mua sau!";
                return RedirectToAction("ProductDetail", new { productID = productID });
            }
            var review = new ProductReView
            {
                userID = userId.Value,
                productID = productID,
                rating = rating,
                comment = comment,
                CreateDate = DateTime.Now,
                orderID = lastOrder.orderID
            };
            _context.ProductReView.Add(review);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Đánh giá của bạn đã được lưu!";
            return RedirectToAction("ProductDetail", new { id = productID });
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
            var products = await _context.Products.Include(p=>p.catalog)
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
