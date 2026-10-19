using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HerSeyBurada.WebMvcUI.Controllers
{
    public class KuponController : Controller
    {
        [HttpPost("kupon/uygula")]
        public IActionResult KuponUygula(string KuponKodu, decimal Tutar)
        {
            if (string.IsNullOrWhiteSpace(KuponKodu))
                return Json(new { basariliMi = false, mesaj = "Lütfen bir kupon kodu giriniz." });

            string kod = KuponKodu.Trim().ToUpper();
            decimal indirim = 0;

            if (kod == "CUMA100")
            {
                if (Tutar < 2000)
                    return Json(new { basariliMi = false, mesaj = "CUMA100 kuponu en az 2.000,00 TL üzeri sepetlerde geçerlidir." });

                indirim = 100;
            }
            else if (kod == "CUMA250")
            {
                if (Tutar < 5000)
                    return Json(new { basariliMi = false, mesaj = "CUMA250 kuponu en az 5.000,00 TL üzeri sepetlerde geçerlidir." });

                indirim = 250;
            }
            else if (kod == "HOŞGELDİN50")
            {
                indirim = 50;
            }
            else
            {
                return Json(new { basariliMi = false, mesaj = "Geçersiz veya süresi dolmuş kupon kodu." });
            }

            decimal yeniTutar = Math.Max(0, Tutar - indirim);

            return Json(new
            {
                basariliMi = true,
                mesaj = $"{indirim:N2} TL kupon indirimi uygulandı!",
                indirimTutari = indirim,
                yeniSepetTutari = yeniTutar
            });
        }
    }
}