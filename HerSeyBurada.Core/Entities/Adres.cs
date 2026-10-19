using System;
using HerSeyBurada.Core.Concrete;

namespace HerSeyBurada.Core.Entities
{
    public class Adres : BaseEntity
    {
        public Guid KullaniciId { get; set; }
        public string AdresBasligi { get; set; } // Ev, İş vb.
        public string AdSoyad { get; set; }
        public string Telefon { get; set; }
        public string Sehir { get; set; }
        public string Ilce { get; set; }
        public string AcikAdres { get; set; }
        public bool KurumsalMi { get; set; } = false;
        public string FirmaAdı { get; set; }
        public string VergiNo { get; set; }

        public virtual Kullanici Kullanici { get; set; }
    }
}