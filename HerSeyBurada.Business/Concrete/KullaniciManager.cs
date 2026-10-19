using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HerSeyBurada.Business.Abstract;
using HerSeyBurada.Business.Models;
using HerSeyBurada.Business.Security;
using HerSeyBurada.Core.Entities;
using HerSeyBurada.DataAccess.Abstract;
using HerSeyBurada.DataAccess.Concrete;

namespace HerSeyBurada.Business.Concrete
{
    public class KullaniciManager : IKullaniciService
    {
        private readonly IRepository<Kullanici> _kullaniciRepository;
        private readonly IRepository<Kupon> _kuponRepository;
        private readonly HerSeyBuradaDbContext _context;

        public KullaniciManager(IRepository<Kullanici> kullaniciRepository, IRepository<Kupon> kuponRepository, HerSeyBuradaDbContext context)
        {
            _kullaniciRepository = kullaniciRepository;
            _kuponRepository = kuponRepository;
            _context = context;
        }

        public async Task<(bool Basarili, string Mesaj, string Token)> KayitOlAsync(KayitDto dto)
        {
            var varMi = (await _kullaniciRepository.FindAsync(x => x.Eposta == dto.Eposta)).FirstOrDefault();
            if (varMi != null)
                return (false, "Bu e-posta adresi ile zaten bir hesap mevcut.", null);

            var yeniKullanici = new Kullanici
            {
                Ad = dto.Ad,
                Soyad = dto.Soyad,
                Eposta = dto.Eposta,
                Telefon = dto.Telefon,
                Rol = "Musteri",
                SifreHash = SifreHashle(dto.Sifre)
            };

            await _kullaniciRepository.AddAsync(yeniKullanici);

            // TREDNYOL MANTIĞI: İlk üyelikte 150 TL Hoş Geldin Kuponu oluştur
            var ilkUyelikKuponu = new Kupon
            {
                Kod = $"HOSGELDIN150-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}",
                IndirimTutari = 150.00m,
                MinimumSepetTutari = 500.00m,
                BitisTarihi = DateTime.UtcNow.AddDays(30),
                IlkUyelikKuponuMu = true
            };
            await _kuponRepository.AddAsync(ilkUyelikKuponu);

            await _context.SaveChangesAsync();

            var token = TokenHandler.TokenOlustur(yeniKullanici);
            return (true, "Üyelik kaydı başarıyla tamamlandı. 150 TL kuponunuz hesabınıza tanımlandı!", token);
        }

        public async Task<(bool Basarili, string Mesaj, string Token, Kullanici Kullanici)> GirisYapAsync(GirisDto dto)
        {
            var hashSifre = SifreHashle(dto.Sifre);
            var kullanici = (await _kullaniciRepository.FindAsync(x => x.Eposta == dto.Eposta && x.SifreHash == hashSifre)).FirstOrDefault();

            if (kullanici == null)
                return (false, "E-posta veya şifre hatalı.", null, null);

            var token = TokenHandler.TokenOlustur(kullanici);
            return (true, "Giriş başarılı.", token, kullanici);
        }

        public async Task<(bool Basarili, string Mesaj)> SifremiUnuttumTokenOlusturAsync(string eposta)
        {
            var kullanici = (await _kullaniciRepository.FindAsync(x => x.Eposta == eposta)).FirstOrDefault();
            if (kullanici == null)
                return (false, "Sistemde kayıtlı e-posta adresi bulunamadı.");

            kullanici.SifreSifirlamaToken = Guid.NewGuid().ToString("N");
            kullanici.TokenGecerlilikTarihi = DateTime.UtcNow.AddHours(2);

            _kullaniciRepository.Update(kullanici);
            await _context.SaveChangesAsync();

            // Gerçek projede burada SMTP ile e-posta gönderilir
            return (true, $"Şifre sıfırlama bağlantısı oluşturuldu. Token: {kullanici.SifreSifirlamaToken}");
        }

        public async Task<(bool Basarili, string Mesaj)> SifreSifirlaAsync(SifreSifirlaDto dto)
        {
            var kullanici = (await _kullaniciRepository.FindAsync(x => x.SifreSifirlamaToken == dto.Token && x.TokenGecerlilikTarihi > DateTime.UtcNow)).FirstOrDefault();
            if (kullanici == null)
                return (false, "Geçersiz veya süresi dolmuş sıfırlama bağlantısı.");

            kullanici.SifreHash = SifreHashle(dto.YeniSifre);
            kullanici.SifreSifirlamaToken = null;
            kullanici.TokenGecerlilikTarihi = null;

            _kullaniciRepository.Update(kullanici);
            await _context.SaveChangesAsync();

            return (true, "Şifreniz başarıyla güncellendi.");
        }

        private string SifreHashle(string sifre)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(sifre));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}