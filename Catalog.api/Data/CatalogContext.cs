using Catalog.api.Entities;
using MongoDB.Driver;

namespace Catalog.api.Data
{
    public class CatalogContext : ICatalogContext
    {
        private readonly IConfiguration configuration;

        public IMongoCollection<Product> Products {get; set;}

        public CatalogContext(IConfiguration configuration)
        {
            this.configuration = configuration;
            var client = new MongoClient(
                this.configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var database = client.GetDatabase(
                this.configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

            Products = database.GetCollection<Product>(
                this.configuration.GetValue<string>("DatabaseSettings:CollectionName"));
        }
    }
}
