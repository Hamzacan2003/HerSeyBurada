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
        public Task EmailGonderAsync(string aliciEposta, string konu, string icerikHtml)
        {
            // Sayfanın dönüp kilitlenmemesi için arka planda tetikliyoruz
            _ = Task.Run(async () =>
            {
                try
                {
                    string smtpHost = "smtp.gmail.com";
                    int smtpPort = 587;

                    string gonderenEposta = "hamzacana98@gmail.com";
                    string gonderenSifre = "agjrpgigukkivbeu";

                    using (var client = new SmtpClient(smtpHost, smtpPort))
                    {
                        client.Credentials = new NetworkCredential(gonderenEposta, gonderenSifre);
                        client.EnableSsl = true;

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress(gonderenEposta, "HerSeyBurada Pazaryeri"),
                            Subject = konu,
                            Body = icerikHtml,
                            IsBodyHtml = true
                        };

                        mailMessage.To.Add(aliciEposta);

                        await client.SendMailAsync(mailMessage);
                        Console.WriteLine($"[GMAIL SMTP BASARILI]: {aliciEposta} adresine mail atıldı.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GMAIL SMTP HATA]: {ex.Message}");
                }
            });

            return Task.CompletedTask;
        }
    }
}