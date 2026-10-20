using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _client = new HttpClient();
            _configuration = configuration;
            _logger = logger;
        }

        public async Task EmailGonderAsync(string aliciEposta, string konu, string icerikHtml)
        {
            try
            {
                // 1. API Key'i hem Environment hem Configuration (Render / appsettings) üzerinden ara
                string apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY")
                               ?? _configuration["BREVO_API_KEY"]
                               ?? _configuration["BrevoSettings:ApiKey"];

                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogError("==================================================");
                    _logger.LogError("[BREVO HATA]: BREVO_API_KEY ortam değişkenlerinde bulunamadı!");
                    _logger.LogError("Lütfen Render Dashboard > Environment kısmında BREVO_API_KEY eklendiğinden emin olun.");
                    _logger.LogError("==================================================");
                    return;
                }

                // 2. Brevo API Payload Oluşturma
                var requestBody = new
                {
                    sender = new { name = "HerŞeyBurada", email = "hamzacana98@gmail.com" },
                    to = new[] { new { email = aliciEposta } },
                    subject = konu,
                    htmlContent = icerikHtml
                };

                var jsonPayload = JsonSerializer.Serialize(requestBody);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");

                request.Headers.Add("api-key", apiKey.Trim());
                request.Headers.Add("accept", "application/json");
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // 3. İsteği Gönder ve Yanıtı Yakala
                var response = await _client.SendAsync(request);
                string responseDetails = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("==================================================");
                    _logger.LogInformation($"[BREVO BAŞARILI]: {aliciEposta} adresine e-posta gönderildi.");
                    _logger.LogInformation($"Brevo Yanıtı: {responseDetails}");
                    _logger.LogInformation("==================================================");
                }
                else
                {
                    _logger.LogError("==================================================");
                    _logger.LogError($"[BREVO HATA RESP]: Status: {response.StatusCode}");
                    _logger.LogError($"[BREVO HATA DETAYI]: {responseDetails}");
                    _logger.LogError("==================================================");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("==================================================");
                _logger.LogError($"[BREVO EXCEPTION]: E-posta gönderiminde sistem hatası oluştu: {ex.Message}");
                _logger.LogError("==================================================");
            }
        }
    }
}