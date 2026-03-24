using System.Text;
using System.Text.Json;
using InventoryApp.Mobile.Models;

namespace InventoryApp.Mobile.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7115/";

        public AuthApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Auth/register", content);
            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<AuthResponse>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Auth/login", content);
            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<AuthResponse>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}