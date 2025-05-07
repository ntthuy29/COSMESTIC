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
    public class ProductController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly IWebHostEnvironment _environment;

        public ProductController(AppDbContext dbContext, IWebHostEnvironment environment)
        {
            this.dbContext = dbContext;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string search, string danhmuc)
        {
            var viewModel = new ProductViewModel
            {
                TuKhoaTimKiem = search,
                DanhMucDuocChon = danhmuc,
                DanhMucs = await dbContext.Catalogs
                .Select(c => new SelectListItem
                {
                    Value = c.catalogID.ToString(),
                    Text = c.catalogName
                })
                .ToListAsync()
            };

            // Build query for products
            var query = dbContext.Products
                .Include(p => p.catalog)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.productName.Contains(search) ||
                                       p.productDescription.Contains(search));
            }

            // Apply category filter
            if (!string.IsNullOrEmpty(danhmuc) && int.TryParse(danhmuc, out int catalogId))
            {
                query = query.Where(p => p.catalogID == catalogId);
            }

            // Map to ListProductModel
            viewModel.SanPhams = await query
                .Select(p => new ListProductModel
                {
                    productID = p.productID,
                    productName = p.productName,
                    productDescription = p.productDescription,
                    price = p.price,
                    imagePath = p.imagePath,
                    catalog = p.catalog.catalogName
                })
                .ToListAsync();

            return View(viewModel);
        }
        
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var viewModel = new CreateProduct
            { 
                DanhMucs = await dbContext.Catalogs
                .Select(c => new SelectListItem
                {
                    Value = c.catalogID.ToString(),
                    Text = c.catalogName
                })
                .ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProduct model)
        {
            if (!ModelState.IsValid)
            {
                // Tải lại danh sách dropdown nếu validation thất bại
                model.DanhMucs = await dbContext.Catalogs
                    .Select(c => new SelectListItem
                    {
                        Value = c.catalogID.ToString(),
                        Text = c.catalogName
                    })
                    .ToListAsync();
                return View(model);
            }

            // Xử lý upload file ảnh
            string imagePath = null;
            if (model.imageFile != null && model.imageFile.Length > 0)
            {
                // Định nghĩa thư mục lưu file ảnh (wwwroot/Uploads)
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại
                }

                // Tạo tên file duy nhất bằng Guid + phần mở rộng của file gốc
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file ảnh vào thư mục Uploads
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.imageFile.CopyToAsync(fileStream);
                }

                // Gán đường dẫn tương đối để lưu vào CSDL (ví dụ: /Uploads/abc123.jpg)
                imagePath = "/Uploads/" + uniqueFileName;
            }

            // Ánh xạ view model sang entity
            var product = new Products
            {
                productName = model.productName,
                productDescription = model.productDescription ?? "",
                price = model.price,
                imagePath = imagePath, // Lưu đường dẫn vào CSDL
                catalogID = model.DanhMucDuocChon
            };

            // Lưu vào cơ sở dữ liệu
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index"); // Điều chỉnh theo trang bạn muốn chuyển hướng
        }
    }
}
