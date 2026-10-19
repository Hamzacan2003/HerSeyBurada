using System;
using System.Collections.Generic;

namespace HerSeyBurada.WebMvcUI.Models
{
    public class UrunVaryantViewModel
    {
        public string Beden { get; set; } = string.Empty;
        public string Renk { get; set; } = string.Empty;
        public int Stok { get; set; }
    }

    public class DetayModel
    {
        public Guid Id { get; set; }
        public string Ad { get; set; } = string.Empty;
        public string Magaza { get; set; } = string.Empty;
        public decimal PiyasaFiyati { get; set; }
        public decimal SatisFiyati { get; set; }
        public string GörselUrl { get; set; } = string.Empty;
        public string Kategori { get; set; } = string.Empty; // <-- EKSİK OLAN PROPERTY EKLENDİ
        public List<UrunVaryantViewModel> Varyantlar { get; set; } = new();
    }
}