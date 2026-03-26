namespace InventoryApp.Mobile.Services
{
    public static class ApiConfig
    {
#if ANDROID
        public const string BaseUrl = "https://10.0.2.2:7115/";
#else
        public const string BaseUrl = "https://localhost:7115/";
#endif
    }
}