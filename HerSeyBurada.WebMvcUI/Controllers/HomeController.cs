using Microsoft.AspNetCore.Mvc;

namespace HerSeyBurada.WebMvcUI.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        [HttpGet("Home/Index")]
        public IActionResult Index()
        {
            return RedirectToAction("Liste", "Urun");
        }
    }
}