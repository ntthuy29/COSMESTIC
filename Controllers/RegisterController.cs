using Microsoft.AspNetCore.Mvc;
using COSMESTIC.Models;
using COSMESTIC.Models.Data;

namespace COSMESTIC.Controllers
{
    public class RegisterController : Controller
    {
        private AppDbContext db;
        public RegisterController(AppDbContext _db)
        {
            db = _db;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterModel models)
        {
            if (ModelState.IsValid)
            {
                var ktraUser = db.Users.FirstOrDefault(u => u.email == models.email);
                if(ktraUser != null)
                {
                    TempData["ErrorMessage"] = "Email đã tồn tại";
                    return RedirectToAction("Register");
                }
                var newUser = new Users
                {
                    email = models.email,
                    fullName = models.fullName,
                    phoneNumber = models.phoneNumber,
                    dateOfBirth = models.dateOfBirth,
                    role = "Customer",
                    status = "Active"
                };
                var newAccount = new Account
                {
                    username = models.username,
                    password = models.password
                };
                newUser.account = newAccount;
                db.Users.Add(newUser);
                db.Accounts.Add(newAccount);
                db.SaveChanges();
            }
            return RedirectToAction("Login", "Login");
        }
    }
}
