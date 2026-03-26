using System.Text.Json;
using InventoryApp.Mobile.Models;

namespace InventoryApp.Mobile.Services
{
    public class AuthApiService : BaseApiService
    {
        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var result = await PostAsync("api/Auth/login", request);

            if (!result.Success)
                return null;

            return JsonSerializer.Deserialize<AuthResponse>(result.Content, _jsonOptions);
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            var result = await PostAsync("api/Auth/register", request);

            if (!result.Success)
                return null;

            return JsonSerializer.Deserialize<AuthResponse>(result.Content, _jsonOptions);
        }
    }
}