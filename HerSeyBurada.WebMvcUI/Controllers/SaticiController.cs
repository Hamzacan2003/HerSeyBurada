using HerSeyBurada.Core.Entities;
using HerSeyBurada.DataAccess.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HerSeyBurada.WebMvcUI.Controllers
{
    public class SaticiController : Controller
    {
        private readonly HerSeyBuradaDbContext _context;

        public SaticiController(HerSeyBuradaDbContext context)
        {
            _context = context;
        }

        [HttpGet("satici/urunler")]
        [HttpGet("satici/urunlerim")]
        public async Task<IActionResult> Urunler()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return RedirectToAction("Girisyap", "Security");

            var eposta = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.Include(k => k.Magaza).FirstOrDefaultAsync(k => k.Eposta.ToLower() == eposta.ToLower());

            if (kullanici?.Magaza == null) return RedirectToAction("Girisyap", "Security");

            var urunler = await _context.Urunler
                .Include(u => u.Varyantlar)
                .Where(u => u.MagazaId == kullanici.Magaza.Id)
                .ToListAsync();

            return View("~/Views/Satici/Urunlerim.cshtml", urunler);
        }

        [HttpGet("satici/urun-ekle")]
        public IActionResult UrunEkle() => View();

        [HttpPost("satici/urun-ekle")]
        public async Task<IActionResult> UrunEklePost(string UrunAdi, string Kategori, decimal Fiyat, List<string> SeciliRenkler, string Aciklama, List<int> Stoklar, List<IFormFile> Resimler)
        {
            var eposta = User.FindFirstValue(ClaimTypes.Email);
            var kullanici = await _context.Kullanicilar.Include(k => k.Magaza).FirstOrDefaultAsync(k => k.Eposta.ToLower() == eposta.ToLower());

            if (kullanici?.Magaza == null) return RedirectToAction("Girisyap", "Security");

            List<string> yuklenenYollar = new List<string>();
            var dosyaListesi = Resimler ?? new List<IFormFile>();

            string klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(klasorYolu)) Directory.CreateDirectory(klasorYolu);

            foreach (var dosya in dosyaListesi)
            {
                if (dosya != null && dosya.Length > 0)
                {
                    string uzanti = Path.GetExtension(dosya.FileName);
                    string dosyaAdi = Guid.NewGuid().ToString() + uzanti;
                    string tamYol = Path.Combine(klasorYolu, dosyaAdi);

                    using (var stream = new FileStream(tamYol, FileMode.Create))
                    {
                        await dosya.CopyToAsync(stream);
                    }
                    yuklenenYollar.Add("/uploads/" + dosyaAdi);
                }
            }

            string birincilGorsel = yuklenenYollar.Any() ? string.Join(",", yuklenenYollar) : "/uploads/default.jpg";

            var yeniUrun = new Urun
            {
                Ad = UrunAdi,
                KategoriAdi = Kategori,
                SatisFiyati = Fiyat,
                Aciklama = Aciklama ?? "",
                GorselUrl = birincilGorsel,
                MagazaId = kullanici.Magaza.Id
            };

            await _context.Urunler.AddAsync(yeniUrun);
            await _context.SaveChangesAsync();

            var varyantListesi = new List<UrunVaryant>();
            var renkler = (SeciliRenkler != null && SeciliRenkler.Any()) ? SeciliRenkler : new List<string> { "Siyah" };

            string[] bedenler;
            if (Kategori == "Giyim")
            {
                bedenler = new string[] { "XS", "S", "M", "L", "XL", "XXL" };
            }
            else if (Kategori == "Ayakkabı")
            {
                bedenler = new string[] { "36", "37", "38", "39", "40", "41", "42", "43", "44", "45" };
            }
            else
            {
                bedenler = new string[] { "Standart" };
            }

            int stokIndex = 0;
            foreach (var r in renkler)
            {
                foreach (var b in bedenler)
                {
                    int girilenStok = (Stoklar != null && Stoklar.Count > stokIndex) ? Stoklar[stokIndex] : 10;
                    varyantListesi.Add(new UrunVaryant
                    {
                        UrunId = yeniUrun.Id,
                        Renk = r,
                        Beden = b,
                        StokMiktari = girilenStok
                    });
                    stokIndex++;
                }
            }

            await _context.UrunVaryantlari.AddRangeAsync(varyantListesi);
            await _context.SaveChangesAsync();

            return RedirectToAction("Urunler");
        }

        [HttpGet("satici/urun-duzenle/{id}")]
        public async Task<IActionResult> UrunDuzenle(Guid id)
        {
            var urun = await _context.Urunler.Include(u => u.Varyantlar).FirstOrDefaultAsync(u => u.Id == id);
            if (urun == null) return NotFound();
            return View(urun);
        }

        [HttpPost("satici/urun-duzenle/{id}")]
        public async Task<IActionResult> UrunDuzenlePost(Guid id, string UrunAdi, string Kategori, decimal Fiyat, List<string> SeciliRenkler, string Aciklama, List<int> Stoklar, List<IFormFile> Resimler)
        {
            var urun = await _context.Urunler.Include(u => u.Varyantlar).FirstOrDefaultAsync(u => u.Id == id);
            if (urun == null) return NotFound();

            urun.Ad = UrunAdi;
            urun.KategoriAdi = Kategori;
            urun.SatisFiyati = Fiyat;
            urun.Aciklama = Aciklama ?? "";

            if (Resimler != null && Resimler.Any(r => r.Length > 0))
            {
                List<string> yuklenenYollar = new List<string>();
                string klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(klasorYolu)) Directory.CreateDirectory(klasorYolu);

                foreach (var dosya in Resimler)
                {
                    if (dosya != null && dosya.Length > 0)
                    {
                        string uzanti = Path.GetExtension(dosya.FileName);
                        string dosyaAdi = Guid.NewGuid().ToString() + uzanti;
                        string tamYol = Path.Combine(klasorYolu, dosyaAdi);

                        using (var stream = new FileStream(tamYol, FileMode.Create))
                        {
                            await dosya.CopyToAsync(stream);
                        }
                        yuklenenYollar.Add("/uploads/" + dosyaAdi);
                    }
                }
                if (yuklenenYollar.Any()) urun.GorselUrl = string.Join(",", yuklenenYollar);
            }

            // ESKİ VARYANTLARI TEMİZLEYİP YENİ KATEGORİ VE RENKLERE GÖRE DİZİ YENİDEN OLUŞTURULUYOR
            _context.UrunVaryantlari.RemoveRange(urun.Varyantlar);
            await _context.SaveChangesAsync();

            var yeniVaryantlar = new List<UrunVaryant>();
            var renkler = (SeciliRenkler != null && SeciliRenkler.Any()) ? SeciliRenkler : new List<string> { "Siyah" };

            string[] bedenler;
            if (Kategori == "Giyim")
            {
                bedenler = new string[] { "XS", "S", "M", "L", "XL", "XXL" };
            }
            else if (Kategori == "Ayakkabı")
            {
                bedenler = new string[] { "36", "37", "38", "39", "40", "41", "42", "43", "44", "45" };
            }
            else
            {
                bedenler = new string[] { "Standart" };
            }

            int stokIndex = 0;
            foreach (var r in renkler)
            {
                foreach (var b in bedenler)
                {
                    int girilenStok = (Stoklar != null && Stoklar.Count > stokIndex) ? Stoklar[stokIndex] : 10;
                    yeniVaryantlar.Add(new UrunVaryant
                    {
                        UrunId = urun.Id,
                        Renk = r,
                        Beden = b,
                        StokMiktari = girilenStok
                    });
                    stokIndex++;
                }
            }

            await _context.UrunVaryantlari.AddRangeAsync(yeniVaryantlar);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Ürün ve tüm beden stokları başarıyla güncellendi!";
            return RedirectToAction("Urunler");
        }
    }
}