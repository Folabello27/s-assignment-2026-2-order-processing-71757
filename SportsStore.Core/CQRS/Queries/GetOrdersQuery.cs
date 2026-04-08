using MediatR;
using SportsStore.Core.Domain.DTOs;

namespace SportsStore.Core.CQRS.Queries;

public record GetOrdersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Status = null) : IRequest<PaginatedOrdersResponse>;

public record PaginatedOrdersResponse(
    List<OrderResponseDto> Orders,
    int TotalCount,
    int Page,
    int PageSize);
