using System;
using HerSeyBurada.Core.Concrete;

namespace HerSeyBurada.Core.Entities
{
    public class UrunYorum : BaseEntity
    {
        public Guid UrunId { get; set; }
        public Guid KullaniciId { get; set; }
        public int Puan { get; set; } // 1-5 Arası
        public string YorumMetni { get; set; }
        public string GorselUrl { get; set; } // Müşterinin yüklediği fotoğraf
        public bool OnaylandiMi { get; set; } = true;

        public virtual Urun Urun { get; set; }
        public virtual Kullanici Kullanici { get; set; }
    }
}