using Basket.api.Entities;

namespace Basket.api.Repositories
{
    public interface IBasketRepository
    {
        Task DeleteBasket(string userName);
        Task<ShoppingCart> GetBasket(string userName);
        Task<ShoppingCart?> UpdateBasket(ShoppingCart shoppingCart);
    }
}
