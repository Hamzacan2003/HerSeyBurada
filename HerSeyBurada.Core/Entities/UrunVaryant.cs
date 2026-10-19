using System;
using HerSeyBurada.Core.Concrete;

namespace HerSeyBurada.Core.Entities
{
    public class UrunVaryant : BaseEntity
    {
        public Guid UrunId { get; set; }
        public string Renk { get; set; } = "Standart";
        public string Beden { get; set; } = "STD";
        public string Barkod { get; set; } = string.Empty;
        public int StokMiktari { get; set; }

        // Hataya sebep olan eksik alan:
        public decimal? EkFiyat { get; set; } = 0m;

        public virtual Urun Urun { get; set; }
    }
}