using System;
using System.Threading.Tasks;
using HerSeyBurada.Business.Models;
using HerSeyBurada.Core.Entities;

namespace HerSeyBurada.Business.Abstract
{
    public interface IKullaniciService
    {
        Task<(bool Basarili, string Mesaj, string Token)> KayitOlAsync(KayitDto dto);
        Task<(bool Basarili, string Mesaj, string Token, Kullanici Kullanici)> GirisYapAsync(GirisDto dto);
        Task<(bool Basarili, string Mesaj)> SifremiUnuttumTokenOlusturAsync(string eposta);
        Task<(bool Basarili, string Mesaj)> SifreSifirlaAsync(SifreSifirlaDto dto);
    }
}