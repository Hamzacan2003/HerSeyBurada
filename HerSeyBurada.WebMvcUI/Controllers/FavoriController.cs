using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HerSeyBurada.DataAccess.Concrete;
using HerSeyBurada.Core.Entities;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HerSeyBurada.WebMvcUI.Controllers
{
    public class FavoriController : Controller
    {
        private readonly HerSeyBuradaDbContext _context;

        public FavoriController(HerSeyBuradaDbContext context)
        {
            _context = context;
        }

        [HttpGet("favori/liste")]
        public IActionResult Liste()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                TempData["Hata"] = "Favorilerinizi görüntülemek için lütfen giriş yapınız.";
                return RedirectToAction("Girisyap", "Security");
            }

            return View();
        }
    }
}