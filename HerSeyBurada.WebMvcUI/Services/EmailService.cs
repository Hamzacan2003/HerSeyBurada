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
                string gonderenEmail = "hamzacana98@gmail.com";
                string sifre = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD");

                if (string.IsNullOrEmpty(sifre))
                {
                    Console.WriteLine("[GMAIL SMTP HATA]: Uygulama şifresi bulunamadı!");
                    return;
                }

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(gonderenEmail, sifre);
                    smtp.EnableSsl = true;

                    var mail = new MailMessage
                    {
                        From = new MailAddress(gonderenEmail, "HerSeyBurada"),
                        Subject = konu,
                        Body = icerikHtml,
                        IsBodyHtml = true
                    };

                    mail.To.Add(aliciEposta);
                    await smtp.SendMailAsync(mail);
                    Console.WriteLine($"[GMAIL SMTP BASARILI]: {aliciEposta} adresine mail atıldı.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GMAIL SMTP HATA]: {ex.Message}");
            }
        }
    }
}