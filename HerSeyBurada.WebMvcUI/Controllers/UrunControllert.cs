using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HerSeyBurada.DataAccess.Concrete;
using HerSeyBurada.Core.Entities;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;

namespace HerSeyBurada.WebMvcUI.Controllers
{
    public class UrunController : Controller
    {
        private readonly HerSeyBuradaDbContext _context;

        // Geçici liste çakışmasını engellemek için statik kalıcı hafıza listesi
        public static List<YorumItem> YorumListesi = new List<YorumItem>();

        public class YorumItem
        {
            public Guid UrunId { get; set; }
            public string KullaniciAd { get; set; }
            public int Puan { get; set; }
            public string Yorum { get; set; }
            public DateTime Tarih { get; set; } = DateTime.Now;
        }

        public UrunController(HerSeyBuradaDbContext context)
        {
            _context = context;
        }

        [HttpGet("urun/liste")]
        public async Task<IActionResult> Liste(string kategori, string q)
        {
            var query = _context.Urunler
                .Include(u => u.Magaza)
                .Include(u => u.Varyantlar)
                .AsQueryable();

            if (!string.IsNullOrEmpty(kategori) && kategori != "Tümü")
                query = query.Where(u => u.KategoriAdi == kategori);

            if (!string.IsNullOrEmpty(q))
                query = query.Where(u => u.Ad.Contains(q) || u.KategoriAdi.Contains(q));

            var urunler = await query.ToListAsync();
            return View(urunler);
        }

        [HttpGet("urun/detay/{id}")]
        public async Task<IActionResult> Detay(Guid id)
        {
            var urun = await _context.Urunler
                .Include(u => u.Magaza)
                .Include(u => u.Varyantlar)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (urun == null) return NotFound();

            ViewBag.VaryantJson = System.Text.Json.JsonSerializer.Serialize(
                urun.Varyantlar.Select(v => new
                {
                    v.Id,
                    v.Renk,
                    v.Beden,
                    v.StokMiktari
                })
            );

            // Bu ürüne ait tüm yorumları filtreleyip yolluyoruz
            ViewBag.Yorumlar = YorumListesi.Where(y => y.UrunId == id).OrderByDescending(y => y.Tarih).ToList();

            return View(urun);
        }

        [HttpPost("urun/yorum-ekle")]
        public async Task<IActionResult> YorumEkle(Guid UrunId, int Puan, string Yorum)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return RedirectToAction("Girisyap", "Security");

            var eposta = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Eposta.ToLower() == eposta.ToLower());

            string isim = kullanici != null ? $"{kullanici.Ad} {kullanici.Soyad}" : User.Identity.Name;

            // Yorum hafızaya ekleniyor
            YorumListesi.Add(new YorumItem
            {
                UrunId = UrunId,
                KullaniciAd = isim,
                Puan = Puan,
                Yorum = Yorum
            });

            TempData["Mesaj"] = "Değerlendirmeniz ve yorumunuz başarıyla kaydedildi ve yayınlandı!";
            return RedirectToAction("Detay", new { id = UrunId });
        }
    }
}