using System;

namespace HerSeyBurada.Business.Models
{
    public class KayitDto
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Eposta { get; set; }
        public string Sifre { get; set; }
        public string Telefon { get; set; }
    }

    public class GirisDto
    {
        public string Eposta { get; set; }
        public string Sifre { get; set; }
    }

    public class SifremiUnuttumDto
    {
        public string Eposta { get; set; }
    }

    public class SifreSifirlaDto
    {
        public string Token { get; set; }
        public string YeniSifre { get; set; }
    }
}