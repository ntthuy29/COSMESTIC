using COSMESTIC.Models;
using COSMESTIC.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;
using System.Linq;

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
        public IActionResult Login(LoginModel models)
        {
            Console.WriteLine("Username: " + models.username);
            Console.WriteLine("Password: " + models.password);

            var ktrauser = db.Accounts.FirstOrDefault(u => u.username == models.username && u.password == models.password);

            if (ktrauser == null)
            {
                // Nếu không tìm thấy tài khoản hoặc mật khẩu không đúng
                TempData["ErrorMessage"] = "Tài khoản hoặc mật khẩu không đúng!";
                return RedirectToAction("Login");
            }
            else if (ktrauser.username == "Ngan123@" && ktrauser.password == "Ngan123@")
            {
               return RedirectToAction("Home", "Admin");
            }
            else
            {
                HttpContext.Session.SetInt32("UserID", ktrauser.userID);
                return RedirectToAction("Product", "Product");
            }
        }
    }
}
