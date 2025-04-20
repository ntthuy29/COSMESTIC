using Microsoft.AspNetCore.Mvc;

namespace COSMESTIC.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
