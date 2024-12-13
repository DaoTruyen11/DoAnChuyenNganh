using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PortfolioWeb.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "AIzaSyCt5Wmqq38UAeNmz2U7miUpKCC6SVr35o0";

        public GeminiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetChatResponse(string prompt)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={_apiKey}";
            var content = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(content);
            var response = await _httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // Trích xuất văn bản từ JSON phản hồi
                using (JsonDocument doc = JsonDocument.Parse(responseContent))
                {
                    var text = doc.RootElement
                                  .GetProperty("candidates")[0]
                                  .GetProperty("content")
                                  .GetProperty("parts")[0]
                                  .GetProperty("text")
                                  .GetString();
                    return text;
                }
            }
            return "Lỗi khi gọi API Gemini";
        }
    }
}