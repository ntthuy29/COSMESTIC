using COSMESTIC.Models;
using COSMESTIC.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace COSMESTIC.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext db;
        public LoginController(AppDbContext _db)
        {
            db = _db;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel models)
        {
            Console.WriteLine("Email: " + models.email);
            Console.WriteLine("Password: " + models.password);

            // Tìm người dùng theo email
            var ktrauser = db.Accounts.Include(u => u.user).FirstOrDefault(u => u.email == models.email);

            // Kiểm tra tài khoản và mật khẩu
            if (ktrauser == null || !BCrypt.Net.BCrypt.Verify(models.password, ktrauser.password))
            {
                TempData["ErrorMessageLogin"] = "Tài khoản hoặc mật khẩu không đúng!";
                return RedirectToAction("Login");
            }

            // Tạo danh sách claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, ktrauser.user.fullName),
        new Claim(ClaimTypes.Role, ktrauser.user.role),
        new Claim("UserID", ktrauser.userID.ToString())
    };

            // Lưu thông tin vào session
            HttpContext.Session.SetInt32("UserID", ktrauser.userID);
            HttpContext.Session.SetString("role", ktrauser.user.role);
            HttpContext.Session.SetString("Username", ktrauser.email);

            // Xác thực đăng nhập
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Nếu là Customer, cập nhật giỏ hàng
            if (ktrauser.user.role == "Customer")
            {
                try
                {
                    var cart = db.ShoppingCart.FirstOrDefault(c => c.userID == ktrauser.userID);
                    int quantity = cart?.totalQuantity ?? 0;
                    HttpContext.Session.SetInt32("CartItemCount", quantity);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi giỏ hàng: " + ex.Message);
                }
            }

            // Điều hướng theo vai trò
            if (ktrauser.user.role=="admin")
            {
                return RedirectToAction("Home", "Admin");
            }
            else if (ktrauser.user.role == "sale")
            {
                return RedirectToAction("Home", "Admin");
            }
            else
            {
                return RedirectToAction("Product", "Product");
            }
        }

        public IActionResult forgetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult forgetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var account = db.Users.Include(u=>u.account).FirstOrDefault(a => a.email == model.Email);
                if (account == null)
                {
                    ModelState.AddModelError("", "Email không tồn tại trong hệ thống.");
                    return View(model);
                }
                // Mã hóa mật khẩu mới
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                // Cập nhật mật khẩu trong cơ sở dữ liệu
                account.account.password = hashedPassword;
                db.SaveChanges();

                TempData["Success"] = "Mật khẩu đã được cập nhật.";
                return RedirectToAction("Login");
            }

            return View(model);
        }
    }
   
}
