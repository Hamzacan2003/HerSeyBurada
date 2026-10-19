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
                string smtpHost = "smtp-relay.brevo.com";
                int smtpPort = 587;

                // Brevo SMTP Login kullanıcın (Ekranındaki Login adresi)
                string smtpUser = "bbdfd0001@smtp-brevo.com";

                // Aldığın yeni SMTP Key
                string smtpPass = "xsmtpsib-0875ec8713ea444a1d49376de0d65d0b20562212051552f8d91375bfe0cb8149-YYLjr26xY2aZ4HsL";

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("hamzacana98@gmail.com", "HerSeyBurada Pazaryeri"),
                        Subject = konu,
                        Body = icerikHtml,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(aliciEposta);

                    await client.SendMailAsync(mailMessage);
                    Console.WriteLine($"[BREVO SMTP BASARILI]: {aliciEposta} adresine mail atıldı.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BREVO SMTP HATA]: {ex.Message}");
            }
        }
    }
}