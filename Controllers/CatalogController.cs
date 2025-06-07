using COSMESTIC.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace COSMESTIC.Controllers
{
    public class CatalogController : Controller
    {
        private readonly AppDbContext db;
        private readonly IWebHostEnvironment _environment;
        public CatalogController(AppDbContext _db, IWebHostEnvironment environment)
        {
            db = _db;
            _environment = environment;
        }
        public IActionResult Index()
        {
            var catalogs = db.Catalogs
                .Include(catalogs => catalogs.products) // viết đúng tên navigation property
                .ToList();

            return View(catalogs);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CatalogWithProductsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tạo danh mục mới
            var catalog = new Catalogs
            {
                catalogName = model.catalogName,
                catalogDescription = model.catalogDescription,
                products = new List<Products>()
            };

            // Duyệt qua từng sản phẩm được thêm
            foreach (var p in model.Products)
            {
                string imageRelativePath = null;

                if (p.imageFile != null && p.imageFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var ext = Path.GetExtension(p.imageFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(ext))
                    {
                        ModelState.AddModelError("", "Chỉ chấp nhận file ảnh (.jpg, .png, .gif)");
                        return View(model);
                    }

                    // Tạo tên file duy nhất và lưu vào thư mục wwwroot/Img
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "Img");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + ext;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await p.imageFile.CopyToAsync(fileStream);
                    }

                    imageRelativePath = "Img/" + uniqueFileName;
                }

                // Thêm sản phẩm vào danh mục
                catalog.products.Add(new Products
                {
                    
                    productName = p.productName,
                    productDescription = p.productDescription,
                    price = p.price,
                    quantity = p.quantity,
                    imagePath = imageRelativePath
                });
            }

            // Lưu vào cơ sở dữ liệu
            db.Catalogs.Add(catalog);
            await db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            var catalog = db.Catalogs
                .Include(c => c.products)
                .FirstOrDefault(c => c.catalogID == id);

            if (catalog == null)
            {
                // Có thể chuyển hướng hoặc trả về view "Không tìm thấy"
                TempData["ErrorMessage"] = "Không tìm thấy danh mục.";
                return RedirectToAction("Index"); // hoặc trả về view riêng
            }

            return View(catalog);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var catalog = db.Catalogs
                .Include(c => c.products) // Nạp thêm danh sách products
                .FirstOrDefault(c => c.catalogID == id);

            if (catalog == null)
            {
                TempData["ErrorMessageDeleteCatalog"] = "Không tìm thấy danh mục được yêu cầu";
                return RedirectToAction("Index");
            }

            // Kiểm tra nếu danh mục còn sản phẩm
            if (catalog.products != null && catalog.products.Any())
            {
                TempData["ErrorMessageDeleteCatalog"] = "Không thể xóa danh mục vì vẫn còn sản phẩm liên kết.";
                return RedirectToAction("Index");
            }

            db.Catalogs.Remove(catalog);
            await db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa danh mục thành công.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var catalog = db.Catalogs
                .Include(c => c.products)
                .FirstOrDefault(c => c.catalogID == id);

            if (catalog == null) {
                return RedirectToAction("Index");
            }


            var model = new CatalogWithProductsViewModel
            {
                catalogID = catalog.catalogID,
                catalogName = catalog.catalogName,
                catalogDescription = catalog.catalogDescription,
                Products = catalog.products.Select(p => new ProductItemViewModel
                {
                    productID = p.productID,
                    productName = p.productName,
                    productDescription = p.productDescription,
                    price = p.price,
                    quantity = p.quantity,
                    imagePath = p.imagePath
                }).ToList()
            };

            return View(model); // View ở đây dùng CatalogWithProductsViewModel
        }

        [HttpPost]
        public async Task<IActionResult> Edits(CatalogWithProductsViewModel model)
        {
            var catalog = db.Catalogs
                .Include(c => c.products)
                .FirstOrDefault(c => c.catalogID == model.catalogID);

            if (catalog == null)
            {
                
                return NotFound();
            }

            catalog.catalogName = model.catalogName;
            catalog.catalogDescription = model.catalogDescription;

            foreach (var p in model.Products)
            {
                string imageRelativePath = null;

                if (p.imageFile != null && p.imageFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var ext = Path.GetExtension(p.imageFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(ext))
                    {
                        ModelState.AddModelError("", "Chỉ chấp nhận file ảnh (.jpg, .png, .gif)");
                        return View(model);
                    }

                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "Img");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + ext;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await p.imageFile.CopyToAsync(fileStream);
                    }

                    imageRelativePath = "Img/" + uniqueFileName;
                }

                // Cập nhật sản phẩm
                var product = db.Products.FirstOrDefault(pr => pr.productID == p.productID);
                if (product != null)
                {
                    product.productName = p.productName;
                    product.productDescription = p.productDescription;
                    product.price = p.price;
                    product.quantity = p.quantity;

                    // Nếu có ảnh mới thì cập nhật, còn không giữ lại ảnh cũ
                    if (!string.IsNullOrEmpty(imageRelativePath))
                    {
                        product.imagePath = imageRelativePath;
                    }
                }
            }

            await db.SaveChangesAsync();
            TempData["successEdit"] = "Thay đổi thành công";
            return RedirectToAction("Edit", new { id = model.catalogID });

        }


    }
}
