using COSMESTIC.Models.Data;
using COSMESTIC.Models.Discount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace COSMESTIC.Controllers
{
    public class DiscountController : Controller
    {
        private readonly AppDbContext dbContext;
        public DiscountController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]//đây là controller để hiển thị ra danh sách sản phẩm
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
        }
        //làm 2 action là edit và delete là xog @@

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var discount = await dbContext.Discount.FindAsync(id);
            if (discount == null)
            {
                return NotFound();
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

        }

        //Controller delete này chưa hoạt động được vì còn phải chờ bảng đơn hàng seed dữ liệu để kiểm tra 
        public async Task<IActionResult> Delete(int id)
        {
            var art = await dbContext.Discount.FindAsync(id);
            if (art != null)
            {
                dbContext.Discount.Remove(art);
                await dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Bài viết đã được xóa thành công";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}
