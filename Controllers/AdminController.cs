using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace COSMESTIC.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles = "admin")]
        public IActionResult Home()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        public IActionResult Product()
        {
            return RedirectToAction("Index", "AdminProduct");
        }
        [Authorize(Roles = "admin")]
        public IActionResult Discount()
        {
            return RedirectToAction("Index", "Discount");
        }
        [Authorize(Roles = "admin")]
        public IActionResult User()
        {
            return RedirectToAction("Index", "User");
        }
        [Authorize(Roles = "admin")]
        public IActionResult Order()
        {
            return RedirectToAction("IndexAdminOrder", "Order");
        }
        public IActionResult Revenue()
        {
            return RedirectToAction("mainRevenue", "Revenue");
        }
    }
}
