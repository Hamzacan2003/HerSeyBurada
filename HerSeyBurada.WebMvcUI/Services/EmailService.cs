using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HerSeyBurada.WebMvcUI.Services
{
    public interface IEmailService
    {
        Task EmailGonderAsync(string aliciEposta, string konu, string icerikHtml);
    }

    public class EmailService : IEmailService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task EmailGonderAsync(string aliciEposta, string konu, string icerikHtml)
        {
            try
            {
                // API Anahtarını Render veya sistem ortam değişkenlerinden alıyoruz
                string apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY");

                if (string.IsNullOrEmpty(apiKey))
                {
                    Console.WriteLine("[BREVO EMAIL HATA]: BREVO_API_KEY bulunamadı!");
                    return;
                }

                var payload = new
                {
                    sender = new { name = "HerSeyBurada Pazaryeri", email = "hamzacana98@gmail.com" },
                    to = new[] { new { email = aliciEposta } },
                    subject = konu,
                    htmlContent = icerikHtml
                };

                string jsonPayload = JsonSerializer.Serialize(payload);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");

                request.Headers.Add("api-key", apiKey);
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[BREVO EMAIL GERCEK BASARILI]: {aliciEposta} adresine gönderildi.");
                }
                else
                {
                    Console.WriteLine($"[BREVO EMAIL HATA]: {response.StatusCode} - {responseBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ISTISNA HATA]: {ex.Message}");
            }
        }
    }
}