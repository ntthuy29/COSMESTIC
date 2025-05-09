using Microsoft.AspNetCore.Mvc;

namespace COSMESTIC.Controllers
{
    public class AdminController : Controller
    {

        public IActionResult Home()
        {
            return View();
        }
        public IActionResult Product()
        {
            return RedirectToAction("Index", "AdminProduct");
        }
        public IActionResult Discount()
        {
            return RedirectToAction("Index", "Discount");
        }
    }
}
