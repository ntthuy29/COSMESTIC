using COSMESTIC.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
            var catalogs = _context.Catalogs.ToList();
            ViewBag.Catalogs = catalogs;

            var products = _context.Products.ToList(); // Lấy tất cả sản phẩm từ DB
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
            if(userId != null)
            {
                hasPurchased = _context.Orders
                                       .Where(o => o.userID == userId && o.status == "Shipped")
                                       .Any(o => o.orderDetails.Any(od => od.productID == id));
            }
            ViewBag.HasPurchased = hasPurchased;
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
            var review = new ProductReView
            {
                userID = userId.Value,
                productID = productID,
                rating = rating,
                comment = comment,
                CreateDate = DateTime.Now
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
