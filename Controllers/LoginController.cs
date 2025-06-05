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
            Console.WriteLine("Username: " + models.username);
            Console.WriteLine("Password: " + models.password);



            var ktrauser = db.Accounts.Include(u => u.user).FirstOrDefault(u => u.username == models.username && u.password == models.password);
            if (ktrauser != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, ktrauser.user.fullName),
            new Claim(ClaimTypes.Role, ktrauser.user.role), // lấy role từ DB
            new Claim("UserID", ktrauser.userID.ToString()) // custom claim nếu cần

        };
                HttpContext.Session.SetInt32("UserID", ktrauser.userID);

                HttpContext.Session.SetString("role", ktrauser.user.role);
                HttpContext.Session.SetString("Username", ktrauser.username);
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            }
            if (ktrauser == null)
            {
                // Nếu không tìm thấy tài khoản hoặc mật khẩu không đúng
                TempData["ErrorMessage"] = "Tài khoản hoặc mật khẩu không đúng!";
                return RedirectToAction("Login");
            }
            if (ktrauser.user.role == "Customer")
            {
                try
                {
                    var cart = db.ShoppingCart.FirstOrDefault(c => c.userID == ktrauser.userID);

                    if (cart != null && cart.totalQuantity != null)
                    {
                        HttpContext.Session.SetInt32("CartItemCount", cart.totalQuantity);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message); // hoặc e.ToString() nếu cần đầy đủ
                    return RedirectToAction("Product", "Product");
                }

            }
            if (ktrauser == null)
            {
                // Nếu không tìm thấy tài khoản hoặc mật khẩu không đúng
                TempData["ErrorMessage"] = "Tài khoản hoặc mật khẩu không đúng!";
                return RedirectToAction("Login");
            }
            else if (ktrauser.username == "Ngan123@" && ktrauser.password == "Ngan123@")
            {
                return RedirectToAction("Home", "Admin");
            }else if (ktrauser.user.role == "sale")
            {
                Console.WriteLine("role = sale");
                return RedirectToAction("Home", "Admin");

            }
            else
            {



                return RedirectToAction("Product", "Product");

            }

        }
    }
}
