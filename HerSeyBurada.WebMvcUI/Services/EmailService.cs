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
            await Task.Run(() =>
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

                    sifre = sifre.Replace(" ", "").Trim();

                    using (var mail = new MailMessage())
                    {
                        mail.From = new MailAddress(gonderenEmail, "HerSeyBurada");
                        mail.To.Add(aliciEposta);
                        mail.Subject = konu;
                        mail.Body = icerikHtml;
                        mail.IsBodyHtml = true;

                        // Port 465 (SSL) kullanarak Render port engellerini aşıyoruz
                        using (var smtp = new SmtpClient("smtp.gmail.com", 465))
                        {
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential(gonderenEmail, sifre);
                            smtp.EnableSsl = true;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.Timeout = 15000;

                            smtp.Send(mail);
                            Console.WriteLine($"[GMAIL SMTP BASARILI]: {aliciEposta} adresine mail atıldı.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GMAIL SMTP HATA]: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"[GMAIL DETAY HATA]: {ex.InnerException.Message}");
                    }
                }
            });
        }
    }
}