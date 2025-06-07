using COSMESTIC.Models.Data;
using COSMESTIC.Models.Discount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;

namespace COSMESTIC.Controllers
{
    public class DiscountController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly ILogger<DiscountController> _logger;
        public DiscountController(AppDbContext dbContext, ILogger<DiscountController> logger)
        {
            this.dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]//đây là action để hiển thị ra danh sách mã khuyến mãi
        public async Task<IActionResult> Index(string search, string duocsudung, string value)
        {
           
                var query = dbContext.Discount.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.discountType.Contains(search));
            }

          
            if (!string.IsNullOrEmpty(duocsudung))
            {
                bool isActive = duocsudung.ToLower() == "duocsudung";
                query = query.Where(d => (d.isActive && isActive) || (d.isActive && !isActive));
            }

            if (!string.IsNullOrEmpty(value))
            {
                if (value == ">10")
                {
                    query = query.Where(d => d.value > 10);
                }
                else if (value == ">20")
                {
                    query = query.Where(d => d.value > 20);
                }
            }
            var model = await query
                .Select(d => new ListDiscountModel
                {
                    discountID = d.discountID,
                    name = d.discountType,
                    value = d.value,
                    startDate = d.startDate.ToString("yyyy-MM-dd"),
                    endDate = d.endDate.ToString("yyyy-MM-dd"),
                    isActive = d.isActive // Giả sử isActive đã được ánh xạ từ kiểu bit sang string
                })
                .ToListAsync();
            ViewData["Search"] = search;
            ViewData["StatusFilter"] = duocsudung; // Sử dụng duocsudung làm statusFilter
            ViewData["ValueFilter"] = value;

            return View(model);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            
            ViewBag.Trangthai = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Kích hoạt" },
                new SelectListItem { Value = "false", Text = "Không kích hoạt" }

            };
            return View();
        }
       

        [HttpPost]
        public async Task<IActionResult> Create(CreateModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Trangthai = new List<SelectListItem>
        {
            new SelectListItem { Value = "true", Text = "Kích hoạt" },
            new SelectListItem { Value = "false", Text = "Không kích hoạt" }
        };
                return View(model); // Trả về view với lỗi validation
            }

            // Kiểm tra thời gian không hợp lệ
            if (model.endDate <= model.startDate)
            {
                ModelState.AddModelError("endDate", "Ngày kết thúc phải sau ngày bắt đầu.");
                ViewBag.Trangthai = new List<SelectListItem>
        {
            new SelectListItem { Value = "true", Text = "Kích hoạt" },
            new SelectListItem { Value = "false", Text = "Không kích hoạt" }
        };
                return View(model);
            }

            var discount = new Discount
            {
                discountType = model.name, // Lưu ý: discountType nên là một enum hoặc giá trị cố định, không dùng name trực tiếp
                value = model.value,
                startDate = model.startDate,
                endDate = model.endDate,
                isActive = model.isActive
            };

            await dbContext.Discount.AddAsync(discount);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        /*[HttpPost]
        public async Task<IActionResult> Create(CreateModel model)
        {
            var discount = new Discount
            {
                discountType = model.name,
                value = model.value,
                startDate = model.startDate,
                endDate = model.endDate,
                isActive = model.isActive
            };

            await dbContext.Discount.AddAsync(discount);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }*/

        //làm 2 action là edit và delete là xog @@

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            //var discount = await dbContext.Discount.FindAsync(id);
            var discount = await dbContext.Discount
        .Include(d => d.Orders) // Tải danh sách Orders
        .FirstOrDefaultAsync(d => d.discountID == id);
            if (discount == null)
            {
                return NotFound();
            }

            if (discount.Orders.Any())
            {
                TempData["WarningMessage"] = "Mã khuyến mãi này đang được áp dụng cho đơn hàng. Bạn chỉ có thể chỉnh sửa ngày và trạng thái.";
            }


            var model = new CreateModel
            {
                discountID = discount.discountID,
                name = discount.discountType,
                value = discount.value,
                startDate = discount.startDate,
                endDate = discount.endDate,
                isActive = discount.isActive
            };

            ViewBag.Trangthai = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Kích hoạt" },
                new SelectListItem { Value = "false", Text = "Không kích hoạt" }
            };
            return View(model);
        }

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                
                if (model.startDate >= model.endDate)
                {
                    ModelState.AddModelError("endDate", "Ngày kết thúc phải lớn hơn ngày bắt đầu.");
                }

                
                if (!Regex.IsMatch(model.name, @"^[a-zA-Z0-9]+$"))
                {
                    ModelState.AddModelError("name", "Tên mã giảm giá không được chứa ký tự đặc biệt hoặc khoảng trắng.");
                }

              
                if (model.value <= 0)
                {
                    ModelState.AddModelError("value", "Giá trị mã giảm giá phải là số thực lớn hơn 0.");
                }

                if (ModelState.IsValid)
                {
                    var discount = await dbContext.Discount.FindAsync(model.discountID);
                    if (discount == null)
                    {
                        return NotFound();
                    }

                    
                    discount.discountType = model.name;
                    discount.value = model.value;
                    discount.startDate = model.startDate;
                    discount.endDate = model.endDate;
                    discount.isActive = model.isActive;

                    await dbContext.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Chỉnh sửa mã giảm giá thành công!";
                    return RedirectToAction("Index"); 
                }
            }

            // Nếu có lỗi, trả lại View với dữ liệu đã nhập
            ViewBag.Trangthai = new List<SelectListItem>
            {
                new SelectListItem { Value = "true", Text = "Kích hoạt" },
                new SelectListItem { Value = "false", Text = "Không kích hoạt" }
            };
            return View(model);

        }*/
        [HttpPost]
        public async Task<IActionResult> Edit(int id, CreateModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Trangthai = new List<SelectListItem>
        {
            new SelectListItem { Value = "true", Text = "Kích hoạt" },
            new SelectListItem { Value = "false", Text = "Không kích hoạt" }
        };
                return View(model);
            }

            var discount = await dbContext.Discount
                .Include(d => d.Orders)
                .FirstOrDefaultAsync(d => d.discountID == id);

            if (discount == null)
            {
                return NotFound();
            }

            if (discount.Orders.Any())
            {
                TempData["WarningMessage"] = "Mã khuyến mãi này đang được áp dụng cho đơn hàng. Bạn chỉ có thể chỉnh sửa ngày và trạng thái.";
                // Giữ nguyên các trường quan trọng
                model.name = discount.discountType; // Không cho phép thay đổi
                model.value = discount.value; // Không cho phép thay đổi
            }

            if (model.endDate <= model.startDate)
            {
                ModelState.AddModelError("endDate", "Ngày kết thúc phải sau ngày bắt đầu.");
                ViewBag.Trangthai = new List<SelectListItem>
        {
            new SelectListItem { Value = "true", Text = "Kích hoạt" },
            new SelectListItem { Value = "false", Text = "Không kích hoạt" }
        };
                return View(model);
            }

            discount.discountType = model.name;
            discount.value = model.value;
            discount.startDate = model.startDate;
            discount.endDate = model.endDate;
            discount.isActive = model.isActive;

            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Mã khuyến mãi đã được cập nhật thành công.";
            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            var discount = await dbContext.Discount
        .Include(d => d.Orders)
        .FirstOrDefaultAsync(d => d.discountID == id);

            if (discount == null)
            {
                return NotFound();
            }

            if (discount.Orders.Any())
            {
                TempData["ErrorMessage"] = "Mã khuyến mãi này đang được áp dụng cho đơn hàng. Không thể xóa.";
                return RedirectToAction("Index");
            }

            dbContext.Discount.Remove(discount);
            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mã khuyến mãi đã được xóa thành công.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ChooseDiscount(string search, string duocsudung, string value, int? productId = null, int? quantity = null, string address = null, string fullName = null, string phoneNumber = null, string returnUrl = null)
        {
            _logger.LogInformation($"ChooseDiscount called with: productId={productId}, quantity={quantity}, address={address}, fullName={fullName}, phoneNumber={phoneNumber}, returnUrl={returnUrl}");

            // Validate returnUrl
            string defaultUrl = Url.Action("ConfirmOrder", "Order"); // Mặc định là Checkout
            if (!string.IsNullOrEmpty(returnUrl))
            {
                Uri uri;
                if (!Uri.TryCreate(returnUrl, UriKind.RelativeOrAbsolute, out uri))
                {
                    returnUrl = defaultUrl;
                }
                else if (uri.IsAbsoluteUri && !uri.Host.Equals(Request.Host.Host, StringComparison.OrdinalIgnoreCase))
                {
                    returnUrl = defaultUrl;
                }
            }
            else
            {
                returnUrl = defaultUrl;
            }

            // Xác định action type dựa trên returnUrl
            string actionType = returnUrl.Contains("ShippingInformationBuyNow") ? "ShippingInformationBuyNow" : "ConfirmOrder";

            var query = dbContext.Discount.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.discountType.Contains(search));
            }

            if (!string.IsNullOrEmpty(duocsudung))
            {
                bool isActive = duocsudung.ToLower() == "True";
                query = query.Where(d => (d.isActive && isActive) || (d.isActive && !isActive));
            }

            if (!string.IsNullOrEmpty(value))
            {
                if (value == ">10")
                {
                    query = query.Where(d => d.value > 10);
                }
                else if (value == ">20")
                {
                    query = query.Where(d => d.value > 20);
                }
            }

            var model = await query
                .Select(d => new ListDiscountModel
                {
                    discountID = d.discountID,
                    name = d.discountType,
                    value = d.value,
                    startDate = d.startDate.ToString("yyyy-MM-dd"),
                    endDate = d.endDate.ToString("yyyy-MM-dd"),
                    isActive = d.isActive
                })
                .ToListAsync();

            ViewData["Search"] = search;
            ViewData["StatusFilter"] = duocsudung;
            ViewData["ValueFilter"] = value;
            ViewBag.ProductId = productId;
            ViewBag.Quantity = quantity;
            ViewBag.Address = HttpUtility.HtmlDecode(address);
            ViewBag.FullName = HttpUtility.HtmlDecode(fullName);
            ViewBag.PhoneNumber = phoneNumber;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ActionType = actionType; // Lưu action type để sử dụng trong view


            return View(model);
        }


        //còn thiếu controller detail
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await dbContext.Discount
                .Include(d => d.Orders)
                .FirstOrDefaultAsync(d => d.discountID == id);

            if (discount == null)
            {
                return NotFound();
            }

            var model = new DetailViewModel
            {
                discountID = discount.discountID,
                name = discount.discountType,//tên
                value = discount.value,
                startDate = discount.startDate,
                endDate = discount.endDate,
                isActive = discount.isActive,
                discountAmount = discount.discountAmount,
                usageCount = discount.Orders.Count, // Số lượt sử dụng thực tế
            };

            // Tính trạng thái
            if (!discount.isActive)
            {
                model.status = "Tạm dừng (Admin)";
            }
            else if (DateTime.Now < discount.startDate)
            {
                model.status = "Đã lên lịch";
            }
            else if (DateTime.Now > discount.endDate)
            {
                model.status = "Đã hết hạn";
            }
            else
            {
                model.status = "Đang hoạt động";
            }

            return View(model);
        }

    }
}
