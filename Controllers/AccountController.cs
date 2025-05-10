using COSMESTIC.Models;
using COSMESTIC.Models.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace COSMESTIC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Detail()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var user = _context.Users.Find(userId);
            if(user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpGet]
        public IActionResult Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public IActionResult Edit(COSMESTIC.Models.Data.Users updatedUser)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            user.fullName = updatedUser.fullName;
            user.email = updatedUser.email;
            user.phoneNumber = updatedUser.phoneNumber;
            user.dateOfBirth = updatedUser.dateOfBirth;
            _context.SaveChanges();
            return RedirectToAction("Detail");
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var account = _context.Accounts.FirstOrDefault(a => a.userID == userId);
            if (account == null)
            {
                return NotFound();
            }

            if (account.password != oldPassword)
            {
                ModelState.AddModelError("", "Mật khẩu cũ không đúng.");
                return View(); 
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu mới không khớp.");
                return View();
            }

            account.password = newPassword;
            _context.SaveChanges();

            // Lưu thông báo thành công vào ViewData
            ViewData["SuccessMessage"] = "Mật khẩu của bạn đã được thay đổi thành công!";
            return View(); // Trả về view và hiển thị thông báo thành công ngay lập tức
        }
        [HttpPost]
        public IActionResult logout()
        {
            
            HttpContext.Session.Clear();

          
            return RedirectToAction("Product", "Product");
        }


    }
}
