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
        private static readonly HttpClient client = new HttpClient();

        public async Task EmailGonderAsync(string aliciEposta, string konu, string icerikHtml)
        {
            try
            {
                string apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY");

                if (string.IsNullOrEmpty(apiKey))
                {
                    Console.WriteLine("[BREVO HATA]: BREVO_API_KEY bulunamadı!");
                    return;
                }

                var requestBody = new
                {
                    sender = new { name = "HerSeyBurada", email = "hamzacana98@gmail.com" },
                    to = new[] { new { email = aliciEposta } },
                    subject = konu,
                    htmlContent = icerikHtml
                };

                var jsonPayload = JsonSerializer.Serialize(requestBody);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");

                request.Headers.Add("api-key", apiKey);
                request.Headers.Add("accept", "application/json");
                request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[BREVO BASARILI]: {aliciEposta} adresine mail atıldı.");
                }
                else
                {
                    string errorDetails = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[BREVO HATA]: {response.StatusCode} - {errorDetails}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BREVO HATA EXCEPTION]: {ex.Message}");
            }
        }
    }
}