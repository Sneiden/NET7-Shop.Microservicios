using Basket.api.Entities;
using Basket.api.Repositories;
using Inventory.grpc.Protos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Basket.api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        public IBasketRepository basketRepository;
        private readonly ExistanceService.ExistanceServiceClient existanceService;

        public BasketController(IBasketRepository basketRepository, ExistanceService.ExistanceServiceClient existanceService)
        {
            this.basketRepository = basketRepository;
            this.existanceService = existanceService;
        }

        [HttpGet("{userName}")]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await basketRepository.GetBasket(userName);

            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpDelete("{userName}")]
        public async Task<ActionResult> DeleteBasket(string userName)
        {
            await basketRepository.DeleteBasket(userName);
            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart shoppingCart)
        {
            foreach (var item in shoppingCart.Items)
            {
                var existance = await existanceService.CheckExistanceAsync(new ProductRequest { Id = item.ProductId });

                item.Quantity = item.Quantity > existance.ProductQty ? existance.ProductQty : item.Quantity;

            }

            await basketRepository.UpdateBasket(shoppingCart);

            return Ok(shoppingCart);
        }
    }
}
