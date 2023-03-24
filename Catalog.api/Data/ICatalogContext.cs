using Catalog.api.Entities;
using MongoDB.Driver;

namespace Catalog.api.Data
{
    public interface ICatalogContext
    {
        IMongoCollection<Product> Products { get; set; }
    }
}
