using MediatR;
using AutoMapper;
using SportsStore.Core.CQRS.Queries;
using SportsStore.Core.Domain.DTOs;
using SportsStore.Core.Domain.Interfaces;

namespace SportsStore.Core.CQRS.Handlers;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderResponseDto?>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderByIdHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderResponseDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
        return order == null ? null : _mapper.Map<OrderResponseDto>(order);
    }
}
