using System;
using HerSeyBurada.Core.Concrete;

namespace HerSeyBurada.Core.Entities
{
    public class Kupon : BaseEntity
    {
        public string Kod { get; set; } 
        public decimal IndirimTutari { get; set; }
        public decimal MinimumSepetTutari { get; set; }
        public DateTime BitisTarihi { get; set; }
        public bool IlkUyelikKuponuMu { get; set; } = false;
    }
}