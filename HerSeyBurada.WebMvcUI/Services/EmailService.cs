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
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task EmailGonderAsync(string aliciEposta, string konu, string icerikHtml)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    string apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY")
                                   ?? _configuration["RESEND_API_KEY"];

                    if (string.IsNullOrEmpty(apiKey))
                    {
                        _logger.LogError("[RESEND HATA]: RESEND_API_KEY ortam değişkenlerinde bulunamadı!");
                        return;
                    }

                    var requestPayload = new
                    {
                        from = "HerŞeyBurada <onboarding@resend.dev>",
                        to = new[] { aliciEposta },
                        subject = konu,
                        html = icerikHtml
                    };

                    var jsonPayload = JsonSerializer.Serialize(requestPayload);
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");

                    request.Headers.Add("Authorization", $"Bearer {apiKey.Trim()}");
                    request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    var response = await _httpClient.SendAsync(request);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"[RESEND BAŞARILI]: Mail {aliciEposta} adresine başarıyla yollandı. Yanıt: {responseContent}");
                    }
                    else
                    {
                        _logger.LogError($"[RESEND HATA]: Status: {response.StatusCode}, Detay: {responseContent}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"[RESEND EXCEPTION]: Mail gönderim hatası: {ex.Message}");
                }
            });

            await Task.CompletedTask;
        }
    }
}