using InventoryApp.Mobile.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace InventoryApp.Mobile.Services
{
    public class CategoryApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7115/";

        public CategoryApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        public async Task<List<CategoryModel>> GetByOrganizationAsync(Guid organizationId)
        {
            var response = await _httpClient.GetAsync($"api/Category/organization/{organizationId}");

            if (!response.IsSuccessStatusCode)
                return new List<CategoryModel>();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<CategoryModel>>(
                       json,
                       new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new List<CategoryModel>();
        }

        public async Task<bool> CreateAsync(CreateCategoryRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Category", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(Guid categoryId, UpdateCategoryRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Category/{categoryId}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeactivateAsync(Guid categoryId)
        {
            var response = await _httpClient.DeleteAsync($"api/Category/{categoryId}");
            return response.IsSuccessStatusCode;
        }
    }
}