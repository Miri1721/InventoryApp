using InventoryApp.Mobile.Models;
using System.Text.Json;

namespace InventoryApp.Mobile.Services
{
    public class OrganizationApiService : BaseApiService
    {
        public async Task<List<OrganizationModel>> GetAllAsync()
        {
            var result = await GetAsync<List<OrganizationModel>>("api/Organization");
            return result ?? new List<OrganizationModel>();
        }
    }
}