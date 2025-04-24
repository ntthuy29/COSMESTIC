using Microsoft.AspNetCore.Mvc;

namespace COSMESTIC.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
