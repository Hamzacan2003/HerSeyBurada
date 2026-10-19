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
            string smtpHost = "smtp.gmail.com";
            // Render üzerinde 587 takıldığı için 465 (SSL) kullanıyoruz
            int smtpPort = 465;

            string gonderenEposta = "hamzacana98@gmail.com";
            string gonderenSifre = "agjrpgigukkivbeu";

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.Credentials = new NetworkCredential(gonderenEposta, gonderenSifre);
                client.EnableSsl = true;
                client.Timeout = 8000; // 8 saniye içinde yanıt alamazsa takılı kalmasın, sayfayı dondurmasın

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(gonderenEposta, "HerSeyBurada Pazaryeri"),
                    Subject = konu,
                    Body = icerikHtml,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(aliciEposta);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}