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
                // API Anahtarını Render Ortam Değişkenlerinden (Environment Variable) okuyoruz
                string apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY");

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    Console.WriteLine("[RESEND EMAIL HATA]: RESEND_API_KEY ortam değişkeni bulunamadı!");
                    return;
                }

                var payload = new
                {
                    from = "HerSeyBurada <onboarding@resend.dev>",
                    to = new[] { aliciEposta },
                    subject = konu,
                    html = icerikHtml
                };

                string jsonPayload = JsonSerializer.Serialize(payload);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");

                request.Headers.Add("Authorization", $"Bearer {apiKey}");
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[RESEND EMAIL BASARILI]: {aliciEposta} adresine gönderildi.");
                }
                else
                {
                    Console.WriteLine($"[RESEND EMAIL HATA]: {response.StatusCode} - {responseBody}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL ISTISNA HATA]: {ex.Message}");
            }
        }
    }
}