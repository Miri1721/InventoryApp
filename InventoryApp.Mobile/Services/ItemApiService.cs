using InventoryApp.Mobile.Models;
using System.Net.Http.Json;

namespace InventoryApp.Mobile.Services
{
    public class ItemApiService : BaseApiService
    {
        public async Task<List<ItemModel>> GetByOrganizationAsync(Guid organizationId)
        {
            var result = await GetAsync<List<ItemModel>>($"api/Item/organization/{organizationId}");
            return result ?? new List<ItemModel>();
        }

        public async Task<List<ItemModel>> GetShortageAsync(Guid organizationId)
        {
            var result = await GetAsync<List<ItemModel>>($"api/Item/shortage/{organizationId}");
            return result ?? new List<ItemModel>();
        }

        public async Task<bool> CreateAsync(CreateItemRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Item", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(Guid itemId, UpdateItemRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Item/{itemId}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeactivateAsync(Guid itemId)
        {
            var response = await _httpClient.PutAsync($"api/Item/deactivate/{itemId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReactivateAsync(Guid itemId)
        {
            var response = await _httpClient.PutAsync($"api/Item/reactivate/{itemId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(Guid itemId)
        {
            var response = await _httpClient.DeleteAsync($"api/Item/{itemId}");
            return response.IsSuccessStatusCode;
        }
    }
}