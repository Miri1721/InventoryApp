using InventoryApp.Mobile.Models;
using System.Net.Http.Json;

namespace InventoryApp.Mobile.Services
{
    public class CategoryApiService : BaseApiService
    {
        public async Task<List<CategoryModel>> GetByOrganizationAsync(Guid organizationId)
        {
            var result = await GetAsync<List<CategoryModel>>($"api/Category/organization/{organizationId}");
            return result ?? new List<CategoryModel>();
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(CreateCategoryRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Category", request);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage = errorMessage?.Trim().Trim('"');

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                errorMessage = "Failed to create category.";
            }

            return (false, errorMessage);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(Guid categoryId, UpdateCategoryRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Category/{categoryId}", request);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            errorMessage = errorMessage?.Trim().Trim('"');

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                errorMessage = "Failed to update category.";
            }

            return (false, errorMessage);
        }

        public async Task<bool> DeactivateAsync(Guid categoryId)
        {
            var response = await _httpClient.PutAsync($"api/Category/deactivate/{categoryId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReactivateAsync(Guid categoryId)
        {
            var response = await _httpClient.PutAsync($"api/Category/reactivate/{categoryId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid categoryId)
        {
            var response = await _httpClient.DeleteAsync($"api/Category/{categoryId}");
            return response.IsSuccessStatusCode;
        }
    }
}