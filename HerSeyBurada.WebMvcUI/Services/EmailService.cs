using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HerSeyBurada.WebMvcUI.Services
{
    public interface IEmailService
    {
        Task EmailGonderAsync(string aliciEposta, string konu, string icerikHtml);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task EmailGonderAsync(string aliciEposta, string konu, string icerikHtml)
        {
            // Arka planda donmesini ve HTTP istegini kilitlenmesini engellemek icin Task.Run ile sarmalıyoruz
            _ = Task.Run(() =>
            {
                try
                {
                    string gmailPassword = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD")
                                         ?? _configuration["GMAIL_APP_PASSWORD"]
                                         ?? Environment.GetEnvironmentVariable("BREVO_API_KEY");

                    if (string.IsNullOrEmpty(gmailPassword))
                    {
                        _logger.LogError("[GMAIL HATA]: Gmail Uygulama Şifresi bulunamadı!");
                        return;
                    }

                    gmailPassword = gmailPassword.Replace(" ", "").Trim();
                    string senderEmail = "hamzacana98@gmail.com";

                    using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtpClient.Credentials = new NetworkCredential(senderEmail, gmailPassword);
                        smtpClient.EnableSsl = true;
                        smtpClient.UseDefaultCredentials = false;

                        // 10 saniye icinde cevap vermezse zaman asimina ugrasın, sayfayi kilitletmesin
                        smtpClient.Timeout = 10000;

                        using (var mailMessage = new MailMessage())
                        {
                            mailMessage.From = new MailAddress(senderEmail, "HerŞeyBurada");
                            mailMessage.To.Add(aliciEposta);
                            mailMessage.Subject = konu;
                            mailMessage.Body = icerikHtml;
                            mailMessage.IsBodyHtml = true;

                            smtpClient.Send(mailMessage);
                            _logger.LogInformation($"[GMAIL SMTP BAŞARILI]: Mail {aliciEposta} adresine başarıyla gönderildi.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[GMAIL SMTP HATA]: Mail gönderilemedi: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        _logger.LogError($"[İÇ HATA DETAYI]: {ex.InnerException.Message}");
                    }
                }
            });

            await Task.CompletedTask;
        }
    }
}