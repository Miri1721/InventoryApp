using InventoryApp.Api.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InventoryApp.Api.Services
{
    public class MongoDbService
    {
        public IMongoCollection<User> Users { get; }
        public IMongoCollection<Organization> Organizations { get; }
        public IMongoCollection<Category> Categories { get; }
        public IMongoCollection<Item> Items { get; }
        public IMongoCollection<StockTransaction> StockTransactions { get; }

        public MongoDbService(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);

            Users = mongoDatabase.GetCollection<User>(mongoDbSettings.Value.UsersCollectionName);
            Organizations = mongoDatabase.GetCollection<Organization>(mongoDbSettings.Value.OrganizationsCollectionName);
            Categories = mongoDatabase.GetCollection<Category>(mongoDbSettings.Value.CategoriesCollectionName);
            Items = mongoDatabase.GetCollection<Item>(mongoDbSettings.Value.ItemsCollectionName);
            StockTransactions = mongoDatabase.GetCollection<StockTransaction>(mongoDbSettings.Value.StockTransactionsCollectionName);
        }
    }
}