using Microsoft.AspNetCore.Mvc;

namespace SljemeTimeAttack.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
