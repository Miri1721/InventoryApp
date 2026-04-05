using InventoryApp.Mobile.Models;

namespace InventoryApp.Mobile.Services
{
    public class UserApiService : BaseApiService
    {
        public async Task<List<UserModel>> GetActiveByOrganizationAsync(Guid organizationId)
        {
            var result = await GetAsync<List<UserModel>>($"api/User/organization/{organizationId}");
            return result ?? new List<UserModel>();
        }

        public async Task<List<UserModel>> GetDeletedByOrganizationAsync(Guid organizationId)
        {
            var result = await GetAsync<List<UserModel>>($"api/User/deleted/organization/{organizationId}");
            return result ?? new List<UserModel>();
        }

        public async Task<bool> DeactivateAsync(Guid userId)
        {
            var response = await _httpClient.PutAsync($"api/User/deactivate/{userId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ReactivateAsync(Guid userId)
        {
            var response = await _httpClient.PutAsync($"api/User/reactivate/{userId}", null);
            return response.IsSuccessStatusCode;
        }
    }
}