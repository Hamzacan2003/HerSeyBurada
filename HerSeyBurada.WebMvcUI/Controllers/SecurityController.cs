using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HerSeyBurada.DataAccess.Concrete;
using HerSeyBurada.Core.Entities;
using HerSeyBurada.WebMvcUI.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace HerSeyBurada.WebMvcUI.Controllers
{
    public class SecurityController : Controller
    {
        private readonly HerSeyBuradaDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IServiceProvider _serviceProvider; // HttpContext'ten bağımsız arka plan servis kontrolü için eklendi

        public SecurityController(HerSeyBuradaDbContext context, IEmailService emailService, IServiceProvider serviceProvider)
        {
            _context = context;
            _emailService = emailService;
            _serviceProvider = serviceProvider;
        }

        [HttpGet("security/girisyap")]
        [HttpGet("kullanici/giris")]
        public IActionResult Girisyap() => View();

        [HttpGet("security/kayitol")]
        public IActionResult Kayitol() => View();

        [HttpPost("security/girisyap")]
        public async Task<IActionResult> GirisyapPost(string Eposta, string Sifre)
        {
            if (string.IsNullOrWhiteSpace(Eposta) || string.IsNullOrWhiteSpace(Sifre))
            {
                ViewBag.Hata = "Lütfen e-posta ve şifrenizi giriniz.";
                return View("Girisyap");
            }

            string epostaClean = Eposta.Trim().ToLower();
            string hashSifre = SifreHashle(Sifre.Trim());

            // Admin Otomatik Ekleme
            if (epostaClean == "hamzacana98@gmail.com" && !await _context.Kullanicilar.AnyAsync(u => u.Eposta == epostaClean))
            {
                var adminUser = new Kullanici
                {
                    Ad = "Hamza Can",
                    Soyad = "Altıntop (Admin)",
                    Eposta = "hamzacana98@gmail.com",
                    SifreHash = SifreHashle("hamzcan.1"),
                    Rol = "Admin"
                };
                await _context.Kullanicilar.AddAsync(adminUser);
                await _context.SaveChangesAsync();
            }

            var user = await _context.Kullanicilar
                .Include(u => u.Magaza)
                .FirstOrDefaultAsync(u => u.Eposta.ToLower() == epostaClean && u.SifreHash == hashSifre);

            if (user == null)
            {
                ViewBag.Hata = "Girdiğiniz e-posta veya şifre hatalı!";
                return View("Girisyap");
            }

            // E-posta veya adında "firma" ya da "satici" geçiyorsa otomatik Satici rolüne çek
            if (user.Rol != "Admin" && (epostaClean.Contains("firma") || epostaClean.Contains("satici") || user.Ad.ToLower().Contains("firma")))
            {
                user.Rol = "Satici";
                if (user.Magaza == null)
                {
                    var magaza = new Magaza
                    {
                        KullaniciId = user.Id,
                        MagazaAdi = string.IsNullOrWhiteSpace(user.Ad) ? "Firma Mağazası" : user.Ad,
                        VergiNo = "1234567890",
                        OnaylandiMi = false
                    };
                    await _context.Magazalar.AddAsync(magaza);
                }
                _context.Kullanicilar.Update(user);
                await _context.SaveChangesAsync();
            }

            await OturumAc(user.Eposta, $"{user.Ad} {user.Soyad}", user.Rol, user.Magaza?.MagazaAdi ?? "Müşteri");

            if (user.Rol == "Admin") return RedirectToAction("Basvurular", "Admin");
            if (user.Rol == "Satici") return RedirectToAction("Urunlerim", "Satici");

            return RedirectToAction("Liste", "Urun");
        }

        [HttpPost("security/kayitol")]
        public async Task<IActionResult> KayitOl(string Ad, string Soyad, string Eposta, string Sifre, string Rol, string MagazaAdi, string VergiNo)
        {
            // GELEN TÜM PARAMETRELERİ KONSOLA YAZDIR (HATA TESPİTİ İÇİN)
            Console.WriteLine($"[KAYIT ISTEGI GELDI] -> Ad: '{Ad}', Soyad: '{Soyad}', Eposta: '{Eposta}', Rol: '{Rol}', Magaza: '{MagazaAdi}', VergiNo: '{VergiNo}'");

            if (string.IsNullOrWhiteSpace(Eposta) || string.IsNullOrWhiteSpace(Sifre))
            {
                TempData["Hata"] = "Lütfen e-posta ve şifre alanlarını doldurunuz.";
                return RedirectToAction("Kayitol");
            }

            string epostaClean = Eposta.Trim().ToLower();

            try
            {
                // Veritabanı kontrolü (Daha önce kayıtlı e-posta var mı?)
                var varKullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Eposta.ToLower() == epostaClean);
                if (varKullanici != null)
                {
                    TempData["Hata"] = "Bu e-posta adresi veritabanında zaten kayıtlı! Lütfen farklı bir e-posta giriniz veya giriş yapınız.";
                    return RedirectToAction("Kayitol");
                }

                // 6 Haneli Onay Kodu Üretimi
                string dogrulamaKodu = new Random().Next(100000, 999999).ToString();

                Console.WriteLine("==========================================");
                Console.WriteLine($"[HERSEYBURADA DOGRULAMA KODU]: {dogrulamaKodu} | ALICI: {epostaClean}");
                Console.WriteLine("==========================================");

                // Rol Tespiti: Rol "Satici" geldiyse veya MagazaAdi/VergiNo doluysa FIRMA kabul et
                bool saticiMi = (!string.IsNullOrWhiteSpace(Rol) && Rol.Equals("Satici", StringComparison.OrdinalIgnoreCase)) ||
                                !string.IsNullOrWhiteSpace(MagazaAdi) ||
                                epostaClean.Contains("firma") || epostaClean.Contains("satici");

                string belirlenenRol = saticiMi ? "Satici" : "Musteri";

                // Geçici verileri TempData'ya al
                TempData["Gecici_Ad"] = string.IsNullOrWhiteSpace(Ad) ? "Firma Yetkilisi" : Ad;
                TempData["Gecici_Soyad"] = Soyad ?? "";
                TempData["Gecici_Eposta"] = epostaClean;
                TempData["Gecici_Sifre"] = Sifre;
                TempData["Gecici_Rol"] = belirlenenRol;
                TempData["Gecici_MagazaAdi"] = string.IsNullOrWhiteSpace(MagazaAdi) ? (string.IsNullOrWhiteSpace(Ad) ? "Yeni Mağaza" : Ad) : MagazaAdi;
                TempData["Gecici_VergiNo"] = string.IsNullOrWhiteSpace(VergiNo) ? "1234567890" : VergiNo;
                TempData["Gecici_Kod"] = dogrulamaKodu;

                TempData.Keep();

                string htmlIcerik = $@"
            <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 12px; text-align: center;'>
                <h2 style='color: #ff9900;'>HerSeyBurada'ya Hoş Geldiniz!</h2>
                <p style='color: #555; font-size: 15px;'>Hesabınızı aktif etmek için 6 haneli e-posta doğrulama kodunuz:</p>
                <div style='background-color: #f8f9fa; border: 2px dashed #ff9900; padding: 15px; font-size: 32px; font-weight: bold; letter-spacing: 6px; color: #222; margin: 20px 0;'>
                    {dogrulamaKodu}
                </div>
            </div>";

                // GÜVENLİ ARKA PLAN MAİL GÖNDERİMİ
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                            await emailService.EmailGonderAsync(epostaClean, "E-Posta Doğrulama Kodunuz - HerSeyBurada", htmlIcerik);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[EMAIL SERVICE ARKA PLAN HATA]: {ex.Message}");
                    }
                });

                TempData["Mesaj"] = $"{epostaClean} adresinize 6 haneli doğrulama kodu gönderiliyor.";
                TempData.Keep();
                return RedirectToAction("EpostaDogrula");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[KAYIT OL EXCEPTION]: {ex.Message}");
                TempData["Hata"] = "Kayıt işlemi sırasında bir sorun oluştu: " + ex.Message;
                return RedirectToAction("Kayitol");
            }
        }

        [HttpGet("security/epostadogrula")]
        public IActionResult EpostaDogrula()
        {
            if (TempData["Gecici_Eposta"] != null)
            {
                TempData.Keep();
            }
            return View();
        }

        [HttpPost("security/epostadogrula")]
        public async Task<IActionResult> EpostaDogrulaPost(string Kod)
        {
            string beklenenKod = TempData["Gecici_Kod"]?.ToString();

            if (string.IsNullOrEmpty(beklenenKod) || Kod != beklenenKod)
            {
                TempData["Hata"] = "Girdiğiniz 6 haneli doğrulama kodu geçersiz veya hatalı!";
                TempData.Keep(); // Hatalı kod girişinde verileri korur
                return View("EpostaDogrula");
            }

            string ad = TempData["Gecici_Ad"]?.ToString();
            string soyad = TempData["Gecici_Soyad"]?.ToString();
            string eposta = TempData["Gecici_Eposta"]?.ToString();
            string rawSifre = TempData["Gecici_Sifre"]?.ToString();
            string rol = TempData["Gecici_Rol"]?.ToString() ?? "Musteri";
            string magazaAdi = TempData["Gecici_MagazaAdi"]?.ToString();
            string vergiNo = TempData["Gecici_VergiNo"]?.ToString();

            bool saticiMi = rol == "Satici";
            string finalRol = saticiMi ? "Satici" : "Musteri";

            var yeniKullanici = new Kullanici
            {
                Ad = ad,
                Soyad = soyad,
                Eposta = eposta,
                SifreHash = SifreHashle(rawSifre),
                Rol = finalRol
            };

            await _context.Kullanicilar.AddAsync(yeniKullanici);
            await _context.SaveChangesAsync();

            string mAdi = "Müşteri";
            if (saticiMi)
            {
                mAdi = string.IsNullOrWhiteSpace(magazaAdi) ? (string.IsNullOrWhiteSpace(ad) ? "Yeni Mağaza" : ad) : magazaAdi;

                var yeniMagaza = new Magaza
                {
                    KullaniciId = yeniKullanici.Id,
                    MagazaAdi = mAdi,
                    VergiNo = string.IsNullOrWhiteSpace(vergiNo) ? "1234567890" : vergiNo,
                    VergiDairesi = "Genel",
                    IBAN = "TR000000000000000000000000",
                    OnaylandiMi = false
                };

                await _context.Magazalar.AddAsync(yeniMagaza);
                await _context.SaveChangesAsync();
            }

            await OturumAc(eposta, $"{ad} {soyad}", finalRol, mAdi);

            TempData["Mesaj"] = "E-posta adresiniz doğrulandı ve hesabınız başarıyla oluşturuldu!";
            return RedirectToAction(finalRol == "Satici" ? "Urunlerim" : "Liste", finalRol == "Satici" ? "Satici" : "Urun");
        }

        [HttpGet("security/sifremiunuttum")]
        public IActionResult SifremiUnuttum() => View();

        [HttpPost("security/sifremiunuttum")]
        public async Task<IActionResult> SifremiUnuttumPost(string Eposta, string EskiSifre, string YeniSifre, string YeniSifreTekrar)
        {
            if (YeniSifre != YeniSifreTekrar)
            {
                TempData["Hata"] = "Yeni şifreler birbiriyle uyuşmuyor!";
                return View("SifremiUnuttum");
            }

            var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(k => k.Eposta.ToLower() == Eposta.Trim().ToLower());

            if (kullanici == null || kullanici.SifreHash != SifreHashle(EskiSifre))
            {
                TempData["Hata"] = "Girilen e-posta veya eski şifre bilgisi hatalı!";
                return View("SifremiUnuttum");
            }

            kullanici.SifreHash = SifreHashle(YeniSifre);
            await _context.SaveChangesAsync();

            TempData["Mesaj"] = "Şifreniz başarıyla güncellendi. Yeni şifrenizle giriş yapabilirsiniz.";
            return RedirectToAction("Girisyap");
        }

        private async Task OturumAc(string eposta, string adSoyad, string role, string magazaAdi)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, adSoyad),
                new Claim(ClaimTypes.Email, eposta),
                new Claim(ClaimTypes.Role, role),
                new Claim("Rol", role),
                new Claim("MagazaAdi", magazaAdi)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }

        [HttpGet("security/cikis")]
        [HttpGet("kullanici/cikis")]
        public async Task<IActionResult> Cikis()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Girisyap", "Security");
        }

        private string SifreHashle(string sifre)
        {
            if (string.IsNullOrEmpty(sifre)) return "";
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(sifre));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}