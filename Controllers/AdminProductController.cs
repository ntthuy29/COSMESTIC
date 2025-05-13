using COSMESTIC.Models.Data;
using COSMESTIC.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace COSMESTIC.Controllers
{
    public class AdminProductController : Controller
    {
        
        private readonly AppDbContext dbContext;
        private readonly IWebHostEnvironment _environment;

        public AdminProductController(AppDbContext dbContext, IWebHostEnvironment environment)
        {
            this.dbContext = dbContext;
            _environment = environment;
        }
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(CreateProduct model)
        {
            if (!ModelState.IsValid)
            {
               
                model.DanhMucs = await dbContext.Catalogs
                    .Select(c => new SelectListItem
                    {
                        Value = c.catalogID.ToString(),
                        Text = c.catalogName
                    })
                    .ToListAsync();
                return View(model);
            }

            
            string imagePath = null;
            if (model.imageFile != null && model.imageFile.Length > 0)
            {
               
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Img");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); 
                }

                
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file ảnh vào thư mục Uploads
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.imageFile.CopyToAsync(fileStream);
                }

               
                imagePath = "Img/" + uniqueFileName;
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
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var art = await dbContext.Products.FindAsync(id);
            if (art != null)
            {
                dbContext.Products.Remove(art);
                await dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa thành công";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        // còn thiếu 2 controller edit, detail
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new CreateProduct
            {
                DanhMucDuocChon = product.catalogID, // Giả định có CategoryId trong Product
                productName = product.productName,
                productDescription = product.productDescription,
                price = product.price,
                // imageFile không cần gán vì không gửi file trong GET
                DanhMucs = await dbContext.Catalogs
                    .Select(c => new SelectListItem
                    {
                        Value = c.catalogID.ToString(),
                        Text = c.catalogName
                    }).ToListAsync()
            };
            // Truyền ImagePath qua ViewBag
            ViewBag.ImagePath = product.imagePath;

            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, CreateProduct model)
        {
           

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await dbContext.Products.FindAsync(id);
                    if (product == null)
                    {
                        return NotFound();
                    }

                    product.catalogID = model.DanhMucDuocChon;
                    product.productName = model.productName;
                    product.productDescription = model.productDescription;
                    product.price = model.price;

                    // Xử lý hình ảnh nếu có
                    if (model.imageFile != null && model.imageFile.Length > 0)
                    {
                        var fileName = Path.GetFileName(model.imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Img", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.imageFile.CopyToAsync(stream);
                        }
                        product.imagePath = "Img/" + fileName;
                    }

                    dbContext.Update(product);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Nếu ModelState không hợp lệ, tải lại danh sách danh mục
            model.DanhMucs = await dbContext.Catalogs
                .Select(c => new SelectListItem
                {
                    Value = c.catalogID.ToString(),
                    Text = c.catalogName
                }).ToListAsync();

            return View(model);
        }
        [Authorize(Roles = "admin")]
        private bool ProductExists(int id)
        {
            return dbContext.Products.Any(e => e.productID == id);
        }
    }
}
