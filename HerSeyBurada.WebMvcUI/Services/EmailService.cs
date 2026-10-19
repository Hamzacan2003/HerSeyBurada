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

                    // Gmail 16 haneli koddaki olası boşlukları temizle
                    sifre = sifre.Replace(" ", "").Trim();

                    using (var mail = new MailMessage())
                    {
                        mail.From = new MailAddress(gonderenEmail, "HerSeyBurada");
                        mail.To.Add(aliciEposta);
                        mail.Subject = konu;
                        mail.Body = icerikHtml;
                        mail.IsBodyHtml = true;

                        using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential(gonderenEmail, sifre);
                            smtp.EnableSsl = true;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.Timeout = 10000; // 10 saniye zaman aşımı

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