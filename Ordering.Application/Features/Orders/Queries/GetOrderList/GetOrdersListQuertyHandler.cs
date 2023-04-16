using AutoMapper;
using MediatR;
using Ordering.Application.Contracts;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Queries.GetOrderList
{
    public class GetOrdersListQuertyHandler : IRequestHandler<GetOrderListQuerty, List<OrdersViewModel>>
    {
        private readonly IGenericRepository<Order> repository;
        private readonly IMapper mapper;

        public GetOrdersListQuertyHandler(IGenericRepository<Order> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        async Task<List<OrdersViewModel>> IRequestHandler<GetOrderListQuerty, List<OrdersViewModel>>.Handle(GetOrderListQuerty request, CancellationToken cancellationToken)
        {
            var orders = await repository.GetAsync(x => x.UserName == request.UserName);
            return mapper.Map<List<OrdersViewModel>>(orders);
        }
    }
}
