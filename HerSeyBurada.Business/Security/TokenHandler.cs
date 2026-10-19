using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using HerSeyBurada.Core.Entities;

namespace HerSeyBurada.Business.Security
{
    public static class TokenHandler
    {
        private const string SecretKey = "HerSeyBurada_Pazaryeri_Gizli_Guvenlik_Anahtari_2026_NetCore";

        public static string TokenOlustur(Kullanici kullanici)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
                    new Claim(ClaimTypes.Email, kullanici.Eposta),
                    new Claim(ClaimTypes.Name, $"{kullanici.Ad} {kullanici.Soyad}"),
                    new Claim(ClaimTypes.Role, kullanici.Rol)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}