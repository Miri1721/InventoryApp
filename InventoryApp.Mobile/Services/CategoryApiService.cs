using InventoryApp.Mobile.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace InventoryApp.Mobile.Services
{
    public class CategoryApiService : BaseApiService
    {
        public async Task<List<CategoryModel>> GetByOrganizationAsync(Guid organizationId)
        {
            var result = await GetAsync<List<CategoryModel>>($"api/Category/organization/{organizationId}");
            return result ?? new List<CategoryModel>();
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