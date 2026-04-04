using System.Text.Json;
using InventoryApp.Mobile.Models;

namespace InventoryApp.Mobile.Services
{
    public class StockTransactionApiService : BaseApiService
    {
        public async Task<StockTransactionModel?> CreateAsync(CreateStockTransactionRequest request)
        {
            var result = await PostAsync("api/StockTransaction", request);

            if (!result.Success)
                return null;

            return JsonSerializer.Deserialize<StockTransactionModel>(result.Content, _jsonOptions);
        }

        public async Task<List<StockTransactionModel>> GetByItemAsync(Guid itemId)
        {
            var result = await GetAsync<List<StockTransactionModel>>($"api/StockTransaction/item/{itemId}");
            return result ?? new List<StockTransactionModel>();
        }
    }
}