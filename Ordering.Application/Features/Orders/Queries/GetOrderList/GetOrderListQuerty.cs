using MediatR;

namespace Ordering.Application.Features.Orders.Queries.GetOrderList
{
    internal class GetOrderListQuerty : IRequest<List<OrdersViewModel>>
    {
        public string UserName { get; set; } = null!;
    }
}
