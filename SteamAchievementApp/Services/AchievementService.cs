using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteamAchievementApp.Services
{
    public class AchievementService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://steam2.p.rapidapi.com";

        public AchievementService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GlobalAchievementResponse> GetGlobalAchievementPercentagesAsync(string appId)
        {
            var url = $"{BaseUrl}/globalAchievementPercentagesForApp/{appId}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Update this with your actual RapidAPI key.
            request.Headers.Add("X-RapidAPI-Key", "1c160f3ffbmsh6d1ec7d34f74a8fp159933jsnc521b53f6255");
            request.Headers.Add("X-RapidAPI-Host", "steam2.p.rapidapi.com");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GlobalAchievementResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }

    public class GlobalAchievementResponse
    {
        public AchievementPercentages achievementpercentages { get; set; }
    }

    public class AchievementPercentages
    {
        public List<Achievement> achievements { get; set; }
    }

    public class Achievement
    {
        public string name { get; set; }
        public double percent { get; set; }
    }
}
