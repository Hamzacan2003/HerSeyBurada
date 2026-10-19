using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace HerSeyBurada.WebMvcUI.Services
{
    public interface IEmailService
    {
        Task EmailGonderAsync(string aliciEposta, string konu, string icerikHtml);
    }

    public class EmailService : IEmailService
    {
        public async Task EmailGonderAsync(string aliciEposta, string konu, string icerikHtml)
        {
            try
            {
                // Render 587 SMTP portunu engellediği için 465 SSL kullanıyoruz
                string smtpHost = "smtp.gmail.com";
                int smtpPort = 465;

                string gonderenEposta = "hamzacana98@gmail.com";
                string gonderenSifre = "agjrpgigukkivbeu"; // Google Uygulama Şifreniz

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.Credentials = new NetworkCredential(gonderenEposta, gonderenSifre);
                    client.EnableSsl = true;
                    client.Timeout = 15000; // 15 Saniye Zaman Aşımı

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(gonderenEposta, "HerSeyBurada Pazaryeri"),
                        Subject = konu,
                        Body = icerikHtml,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(aliciEposta);

                    await client.SendMailAsync(mailMessage);
                    Console.WriteLine($"[GMAIL SMTP GERCEK BASARILI]: {aliciEposta} adresine mail ulaştırıldı.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GMAIL SMTP HATA DETAYI]: {ex.Message} - {ex.InnerException?.Message}");
            }
        }
    }
}