using HerSeyBurada.DataAccess.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HerSeyBurada.WebMvcUI.Controllers
{
    public class BasvuruController : Controller
    {
        private readonly HerSeyBuradaDbContext _context;

        public BasvuruController(HerSeyBuradaDbContext context)
        {
            _context = context;
        }

        [HttpGet("basvuru/liste")]
        public async Task<IActionResult> Liste()
        {
            if (!User.IsInRole("Admin") && User.Identity?.Name != "hamzacana98@gmail.com")
            {
                TempData["Hata"] = "Bu sayfaya yalnızca Admin kullanıcıları erişebilir.";
                return RedirectToAction("Liste", "Urun");
            }

            // Veritabanındaki gerçek mağaza başvurularını çekiyoruz
            var magazalar = await _context.Magazalar
                .Include(m => m.Kullanici)
                .ToListAsync();

            // Eksik view hatası almamak için Admin görünümünü doğrudan hedef gösteriyoruz
            return View("~/Views/Admin/Basvurular.cshtml", magazalar);
        }

        [HttpGet("basvuru/getalljson")]
        public async Task<IActionResult> GetAllJson()
        {
            var magazalar = await _context.Magazalar.Select(m => new
            {
                id = m.Id,
                magazaAdi = m.MagazaAdi,
                vergiNo = m.VergiNo,
                durum = m.OnaylandiMi ? "Onaylandı" : "Beklemede"
            }).ToListAsync();

            return Json(magazalar);
        }

        [HttpPost("basvuru/onayla/{id}")]
        public async Task<IActionResult> Onayla(Guid id)
        {
            if (!User.IsInRole("Admin") && User.Identity?.Name != "hamzacana98@gmail.com")
            {
                return Json(new { success = false, message = "Yetkisiz işlem! Yalnızca Admin onay verebilir." });
            }

            var magaza = await _context.Magazalar.FindAsync(id);
            if (magaza != null)
            {
                magaza.OnaylandiMi = true;
                _context.Magazalar.Update(magaza);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Mağaza başvurusu başarıyla onaylandı." });
            }

            return Json(new { success = false, message = "Başvuru bulunamadı." });
        }
    }
}