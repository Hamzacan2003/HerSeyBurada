using System;
using System.Collections.Generic;
using HerSeyBurada.Core.Concrete;

namespace HerSeyBurada.Core.Entities
{
    public class Siparis : BaseEntity
    {
        public Guid KullaniciId { get; set; }
        public Guid TeslimatAdresiId { get; set; }
        public Guid FaturaAdresiId { get; set; }
        public string SiparisNumarasi { get; set; } // HSB-20261014-XXXX
        public decimal ToplamTutar { get; set; }
        public decimal KargoTutari { get; set; }
        public decimal IndirimTutari { get; set; }
        public string SiparisDurumu { get; set; } // Alındı, Hazırlanıyor, Kargoda, Teslim Edildi

        public virtual Kullanici Kullanici { get; set; }
        public virtual ICollection<SiparisDetay> SiparisDetaylari { get; set; }
    }

    public class SiparisDetay : BaseEntity
    {
        public Guid SiparisId { get; set; }
        public Guid UrunVaryantId { get; set; }
        public Guid MagazaId { get; set; } // Hangi satıcının ürünü olduğunu belirtir
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }
        public string SatirDurumu { get; set; } // Kargoya Verildi, İptal, İade

        public virtual Siparis Siparis { get; set; }
        public virtual UrunVaryant UrunVaryant { get; set; }
        public virtual Magaza Magaza { get; set; }
    }
}