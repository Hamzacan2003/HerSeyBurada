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
    public class SiparisDto
    {
        public string AdresBasligi { get; set; }
        public string Sehir { get; set; }
        public string Ilce { get; set; }
        public string AcikAdres { get; set; }
        public string KartSahibi { get; set; }
        public string KartNumarasi { get; set; }
        public string SonKullanma { get; set; }
        public string Cvv { get; set; }
        public List<SiparisUrunItemDto> Urunler { get; set; }
    }

    public class SiparisUrunItemDto
    {
        public Guid UrunId { get; set; }
        public string Renk { get; set; }
        public string Beden { get; set; }
        public int Adet { get; set; }
        public decimal Fiyat { get; set; }
    }

    public class SiparisController : Controller
    {
        private readonly HerSeyBuradaDbContext _context;

        public SiparisController(HerSeyBuradaDbContext context)
        {
            _context = context;
        }

        [HttpGet("siparis/sepet")]
        public IActionResult Sepet() => View();

        [HttpGet("siparis/listem")]
        public async Task<IActionResult> Listem()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Girisyap", "Security");
            }

            var eposta = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Eposta == eposta);

            if (kullanici == null) return RedirectToAction("Girisyap", "Security");

            var siparisler = await _context.Siparisler
                .Include(s => s.SiparisDetaylari)
                    .ThenInclude(sd => sd.UrunVaryant)
                        .ThenInclude(uv => uv.Urun)
                            .ThenInclude(u => u.Magaza)
                .Where(s => s.KullaniciId == kullanici.Id && !s.IsDeleted)
                .OrderByDescending(s => s.OlusturulmaTarihi)
                .ToListAsync();

            return View(siparisler);
        }

        [HttpPost("siparis/satin-al")]
        public async Task<IActionResult> SatinAl([FromBody] SiparisDto dto)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Lütfen önce giriş yapınız." });
            }

            if (dto == null || dto.Urunler == null || !dto.Urunler.Any())
            {
                return Json(new { success = false, message = "Sepetiniz boş." });
            }

            var eposta = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Eposta == eposta);
            if (kullanici == null) return Json(new { success = false, message = "Kullanıcı bulunamadı." });

            decimal toplamTutar = dto.Urunler.Sum(x => x.Fiyat * x.Adet);

            var yeniSiparis = new Siparis
            {
                KullaniciId = kullanici.Id,
                TeslimatAdresiId = Guid.NewGuid(),
                FaturaAdresiId = Guid.NewGuid(),
                SiparisNumarasi = "HSB-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999),
                ToplamTutar = toplamTutar,
                KargoTutari = 0m,
                IndirimTutari = 0m,
                SiparisDurumu = "Hazırlanıyor"
            };

            await _context.Siparisler.AddAsync(yeniSiparis);
            await _context.SaveChangesAsync();

            // SİPARİŞTEKİ TÜM ÜRÜNLER İÇİN ÇOKLU GERÇEK STOK DÜŞÜŞÜ
            foreach (var item in dto.Urunler)
            {
                var varyantlar = await _context.UrunVaryantlari
                    .Include(v => v.Urun)
                    .Where(v => v.UrunId == item.UrunId)
                    .ToListAsync();

                // Renk ve Beden eşleşmesine göre varyant bul
                var varyant = varyantlar.FirstOrDefault(v =>
                    (string.IsNullOrEmpty(item.Renk) || v.Renk.Equals(item.Renk, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(item.Beden) || v.Beden.Equals(item.Beden, StringComparison.OrdinalIgnoreCase))
                ) ?? varyantlar.FirstOrDefault();

                if (varyant != null)
                {
                    // Stok miktarını satın alınan adet kadar eksilt
                    varyant.StokMiktari = Math.Max(0, varyant.StokMiktari - item.Adet);
                    _context.UrunVaryantlari.Update(varyant);

                    var siparisDetay = new SiparisDetay
                    {
                        SiparisId = yeniSiparis.Id,
                        UrunVaryantId = varyant.Id,
                        MagazaId = varyant.Urun.MagazaId,
                        Adet = item.Adet,
                        BirimFiyat = item.Fiyat,
                        SatirDurumu = "Sipariş Alındı"
                    };
                    await _context.SiparisDetaylari.AddAsync(siparisDetay);
                }
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Siparişiniz alındı! Ürün stokları başarıyla düşürüldü.", siparisNo = yeniSiparis.SiparisNumarasi });
        }
    }
}