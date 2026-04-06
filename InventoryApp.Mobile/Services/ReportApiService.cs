namespace InventoryApp.Mobile.Services
{
    public class ReportApiService : BaseApiService
    {
        public async Task<byte[]?> ExportExcelAsync(Guid organizationId)
        {
            var response = await _httpClient.GetAsync($"api/Report/excel/{organizationId}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]?> ExportShortageExcelAsync(Guid organizationId)
        {
            var response = await _httpClient.GetAsync($"api/Report/shortage-excel/{organizationId}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}