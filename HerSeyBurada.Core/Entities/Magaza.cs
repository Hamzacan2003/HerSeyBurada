using System;
using System.Collections.Generic;
using HerSeyBurada.Core.Concrete;

namespace HerSeyBurada.Core.Entities
{
    public class Magaza : BaseEntity
    {
        public Guid KullaniciId { get; set; }
        public string MagazaAdi { get; set; }
        public string VergiNo { get; set; }
        public string VergiDairesi { get; set; }
        public string IBAN { get; set; }
        public bool OnaylandiMi { get; set; } = false;
        public decimal KomisyonOrani { get; set; } = 10.00m; // Varsayılan %10

        public virtual Kullanici Kullanici { get; set; }
        public virtual ICollection<Urun> Urunler { get; set; }
    }
}