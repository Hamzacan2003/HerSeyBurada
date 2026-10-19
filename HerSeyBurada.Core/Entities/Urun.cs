using System;
using System.Collections.Generic;
using HerSeyBurada.Core.Concrete;

namespace HerSeyBurada.Core.Entities
{
    public class Urun : BaseEntity
    {
        public Guid MagazaId { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string KategoriAdi { get; set; } = "Giyim";
        public decimal PiyasaFiyati { get; set; }
        public decimal SatisFiyati { get; set; }
        public string GorselUrl { get; set; } = "/uploads/default.jpg";

        public virtual Magaza Magaza { get; set; }
        public virtual ICollection<UrunVaryant> Varyantlar { get; set; } = new List<UrunVaryant>();
        public virtual ICollection<UrunYorum> Yorumlar { get; set; } = new List<UrunYorum>();
    }
}