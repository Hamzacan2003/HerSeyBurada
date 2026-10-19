using HerSeyBurada.DataAccess.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace HerSeyBurada.WebMvcUI.Controllers
{
    public class AdminController : Controller
    {
        private readonly HerSeyBuradaDbContext _context;

        public AdminController(HerSeyBuradaDbContext context)
        {
            _context = context;
        }

        [HttpGet("admin/basvurular")]
        public async Task<IActionResult> Basvurular()
        {
            var magazalar = await _context.Magazalar
                .Include(m => m.Kullanici)
                .ToListAsync();

            return View("~/Views/Admin/Basvurular.cshtml", magazalar);
        }

        [HttpPost("admin/magaza-onayla")]
        public async Task<IActionResult> MagazaOnayla(Guid id)
        {
            var magaza = await _context.Magazalar.FindAsync(id);
            if (magaza != null)
            {
                magaza.OnaylandiMi = true;
                _context.Magazalar.Update(magaza);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = $"{magaza.MagazaAdi} onaylandı!";
            }
            return RedirectToAction("Basvurular");
        }

        // SATICI VE TÜM ÜRÜNLERİNİ İLİŞKİLİ OLARAK SİLME
        [HttpPost("admin/magaza-sil")]
        public async Task<IActionResult> MagazaSil(Guid id)
        {
            var magaza = await _context.Magazalar
                .Include(m => m.Kullanici)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (magaza != null)
            {
                var urunler = await _context.Urunler.Where(u => u.MagazaId == magaza.Id).ToListAsync();
                _context.Urunler.RemoveRange(urunler);

                _context.Magazalar.Remove(magaza);
                if (magaza.Kullanici != null)
                {
                    _context.Kullanicilar.Remove(magaza.Kullanici);
                }

                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "Satıcı ve ona ait tüm ürünler sistemden tamamen silindi.";
            }

            return RedirectToAction("Basvurular");
        }
    }
}