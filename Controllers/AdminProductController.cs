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

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(int? page, string search, string danhmuc)
        {
            var model = new ProductViewModel
            {
                TuKhoaTimKiem = search ?? "",
                DanhMucDuocChon = danhmuc 
            };

            // Lấy danh sách danh mục
            model.DanhMucs = await dbContext.Catalogs
                .Select(c => new SelectListItem
                {
                    Value = c.catalogID.ToString(),
                    Text = c.catalogName
                }).ToListAsync();

            // Xây dựng truy vấn sản phẩm
            var query = dbContext.Products
                .Include(p => p.catalog)
                .AsQueryable();

            // Lọc theo từ khóa tìm kiếm
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
            query = query.OrderByDescending(p => p.productID);

            // Tính tổng số sản phẩm
            model.TotalItems = await query.CountAsync();

            
            model.CurrentPage = page ?? 1;

          
            model.SanPhams = await query
                .Skip((model.CurrentPage - 1) * model.PageSize)
                .Take(model.PageSize)
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

            return View(model);
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

               
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.imageFile.CopyToAsync(fileStream);
                }


                imagePath = "Img/" + uniqueFileName;
            }

            var product = new Products
            {
                productName = model.productName,
                productDescription = model.productDescription ?? "",
                price = model.price,
                imagePath = imagePath, 
                catalogID = model.DanhMucDuocChon
            };

           
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index"); 
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await dbContext.Products.FindAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("Index");
            }

            // Kiểm tra xem sản phẩm có trong đơn hàng nào không
            bool isInOrder = await dbContext.orderDetails.AnyAsync(od => od.productID == id);
            if (isInOrder)
            {
                TempData["ErrorMessage"] = "Sản phẩm đang có trong đơn hàng, không thể xóa.";
                return RedirectToAction("Index");
            }

            // Nếu không có trong đơn hàng, tiến hành xóa
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Sản phẩm đã được xóa thành công.";
            return RedirectToAction("Index");
        }
     
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

           
            bool isInOrder = await dbContext.orderDetails.AnyAsync(od => od.productID == id);
            ViewBag.IsInOrder = isInOrder;
            ViewBag.ImagePath = product.imagePath;

            var model = new EditProduct
            {
                DanhMucDuocChon = product.catalogID,
                productName = product.productName,
                productDescription = product.productDescription,
                price = product.price,
                DanhMucs = await dbContext.Catalogs
                    .Select(c => new SelectListItem
                    {
                        Value = c.catalogID.ToString(),
                        Text = c.catalogName
                    }).ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
         [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, EditProduct model)
        {
            
            bool isInOrder = await dbContext.orderDetails.AnyAsync(od => od.productID == id);
            ViewBag.IsInOrder = isInOrder;
           
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
                    product.productDescription = model.productDescription;

                    if (!isInOrder)
                    {
                        product.productName = model.productName;
                        product.price = model.price;
                    }

                  
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
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
            
            model.DanhMucs = await dbContext.Catalogs
                .Select(c => new SelectListItem
                {
                    Value = c.catalogID.ToString(),
                    Text = c.catalogName
                }).ToListAsync();
            ViewBag.ImagePath = (await dbContext.Products.FindAsync(id))?.imagePath;
            return View(model);
        }
        private async Task<bool> ProductExists(int id)
        {
            return await dbContext.Products.AnyAsync(e => e.productID == id);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Detail(int id)
        {
            var product = await dbContext.Products
                .Include(p => p.catalog)
                .Include(p => p.orderDetails)
                .FirstOrDefaultAsync(p => p.productID == id);

            if (product == null)
            {
                return NotFound();
            }

            // Tính toán số lượng đã bán từ OrderDetails
            int soldQuantity = product.orderDetails?.Sum(od => od.quantity) ?? 0;

            // Giả sử TotalQuantity là quantity từ Products (hoặc bạn có thể lấy từ nguồn khác)
            int totalQuantity = product.quantity;

            // Tính số lượng còn lại
            int remainingQuantity = totalQuantity - soldQuantity;

            decimal revenue = soldQuantity * product.price;

            // Tạo model DetailProduct và gán thông tin
            var model = new DetailProduct
            {
                ProductID = product.productID,
                ProductName = product.productName,
                ProductDescription = product.productDescription,
                Price = product.price,
                ImagePath = product.imagePath,
                CatalogName = product.catalog?.catalogName,
                TotalQuantity = totalQuantity,
                SoldQuantity = soldQuantity,
                RemainingQuantity = remainingQuantity,
                Revenue = revenue
            };

            return View(model);
        }


    }
}
