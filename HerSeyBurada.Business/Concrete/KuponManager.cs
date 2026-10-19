using System;
using System.Linq;
using System.Threading.Tasks;
using HerSeyBurada.Business.Abstract;
using HerSeyBurada.Business.Models;
using HerSeyBurada.Core.Entities;
using HerSeyBurada.DataAccess.Abstract;

namespace HerSeyBurada.Business.Concrete
{
    public interface IKuponService
    {
        Task<KuponUygulaSonucDto> KuponUygulaAsync(string kuponKodu, decimal mevcutSepetTutari);
    }

    public class KuponManager : IKuponService
    {
        private readonly IRepository<Kupon> _kuponRepository;

        public KuponManager(IRepository<Kupon> kuponRepository)
        {
            _kuponRepository = kuponRepository;
        }

        public async Task<KuponUygulaSonucDto> KuponUygulaAsync(string kuponKodu, decimal mevcutSepetTutari)
        {
            var kupon = (await _kuponRepository.FindAsync(x => x.Kod == kuponKodu.Trim().ToUpper())).FirstOrDefault();

            if (kupon == null)
                return new KuponUygulaSonucDto { BasariliMi = false, Mesaj = "Geçersiz kupon kodu." };

            if (kupon.BitisTarihi < DateTime.UtcNow)
                return new KuponUygulaSonucDto { BasariliMi = false, Mesaj = "Bu kuponun kullanım süresi dolmuştur." };

            if (mevcutSepetTutari < kupon.MinimumSepetTutari)
                return new KuponUygulaSonucDto { BasariliMi = false, Mesaj = $"Bu kupon sadece {kupon.MinimumSepetTutari:N2} TL ve üzeri sepetlerde geçerlidir." };

            decimal indirim = kupon.IndirimTutari;
            decimal yeniTutar = mevcutSepetTutari - indirim;

            return new KuponUygulaSonucDto
            {
                BasariliMi = true,
                Mesaj = $"{indirim:N2} TL tutarındaki indirim kuponu uygulandı!",
                IndirimTutari = indirim,
                YeniSepetTutari = yeniTutar < 0 ? 0 : yeniTutar
            };
        }
    }
}