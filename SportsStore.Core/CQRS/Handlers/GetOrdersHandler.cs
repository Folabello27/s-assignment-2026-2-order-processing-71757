using MediatR;
using AutoMapper;
using SportsStore.Core.CQRS.Queries;
using SportsStore.Core.Domain.DTOs;
using SportsStore.Core.Domain.Interfaces;

namespace SportsStore.Core.CQRS.Handlers;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, PaginatedOrdersResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrdersHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedOrdersResponse> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetOrdersAsync(
            request.Page, request.PageSize, request.Status);
        var totalCount = await _orderRepository.GetOrderCountAsync(request.Status);

        var orderDtos = _mapper.Map<List<OrderResponseDto>>(orders);

        return new PaginatedOrdersResponse(
            orderDtos,
            totalCount,
            request.Page,
            request.PageSize);
    }
}
