using HerSeyBurada.DataAccess.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HerSeyBurada.WebMvcUI.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly HerSeyBuradaDbContext _context;

        public KullaniciController(HerSeyBuradaDbContext context)
        {
            _context = context;
        }

        [HttpGet("kullanici/profil")]
        public async Task<IActionResult> Profil()
        {
            var eposta = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(eposta)) return RedirectToAction("Girisyap", "Security");

            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Eposta.ToLower() == eposta.ToLower());
            if (kullanici == null) return RedirectToAction("Girisyap", "Security");

            return View(kullanici);
        }

        [HttpPost("kullanici/profil")]
        public async Task<IActionResult> ProfilGuncelle(string Ad, string Soyad, string Eposta)
        {
            var epostaMevcut = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Eposta.ToLower() == epostaMevcut.ToLower());

            if (kullanici != null)
            {
                kullanici.Ad = Ad;
                kullanici.Soyad = Soyad ?? "";
                kullanici.Eposta = Eposta;

                _context.Kullanicilar.Update(kullanici);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Profil bilgileriniz başarıyla güncellendi!";
            }

            return RedirectToAction("Profil");
        }
    }
}