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
            try
            {
                // Değişkeni Render Environment veya Configuration üzerinden al
                string gmailPassword = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD")
                                     ?? _configuration["GMAIL_APP_PASSWORD"]
                                     ?? Environment.GetEnvironmentVariable("BREVO_API_KEY");

                if (string.IsNullOrEmpty(gmailPassword))
                {
                    _logger.LogError("[GMAIL HATA]: Gmail Uygulama Şifresi (GMAIL_APP_PASSWORD) bulunamadı!");
                    return;
                }

                // Boşlukları temizle (Google 16 haneli şifreyi 4'erli gruplar halinde verir)
                gmailPassword = gmailPassword.Replace(" ", "").Trim();

                string senderEmail = "hamzacana98@gmail.com";

                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.Credentials = new NetworkCredential(senderEmail, gmailPassword);
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;

                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(senderEmail, "HerŞeyBurada");
                        mailMessage.To.Add(aliciEposta);
                        mailMessage.Subject = konu;
                        mailMessage.Body = icerikHtml;
                        mailMessage.IsBodyHtml = true;

                        await smtpClient.SendMailAsync(mailMessage);
                        _logger.LogInformation($"[GMAIL SMTP BAŞARILI]: Mail {aliciEposta} adresine başarıyla gönderildi.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[GMAIL SMTP EXCEPTION]: Mail gönderimi başarısız oldu: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"[İÇ HATA DETAYI]: {ex.InnerException.Message}");
                }
            }
        }
    }
}