using System;
using System.Collections.Generic;
using HerSeyBurada.Core.Concrete;

namespace HerSeyBurada.Core.Entities
{
    public class Kullanici : BaseEntity
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Eposta { get; set; }
        public string SifreHash { get; set; }
        public string Telefon { get; set; }
        public string Rol { get; set; } // Admin, Satici, Musteri
        public string SifreSifirlamaToken { get; set; }
        public DateTime? TokenGecerlilikTarihi { get; set; }

        public virtual Magaza Magaza { get; set; }
        public virtual ICollection<Adres> Adresler { get; set; }
        public virtual ICollection<Siparis> Siparisler { get; set; }
        public virtual ICollection<UrunYorum> Yorumlar { get; set; }
    }
}