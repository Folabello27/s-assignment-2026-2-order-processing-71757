using MediatR;
using AutoMapper;
using SportsStore.Core.CQRS.Queries;
using SportsStore.Core.Domain.DTOs;
using SportsStore.Core.Domain.Interfaces;

namespace SportsStore.Core.CQRS.Handlers;

public class GetCustomerOrdersHandler : IRequestHandler<GetCustomerOrdersQuery, List<OrderResponseDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetCustomerOrdersHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<List<OrderResponseDto>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetCustomerOrdersAsync(request.CustomerId);
        return _mapper.Map<List<OrderResponseDto>>(orders);
    }
}
