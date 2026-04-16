using MediatR;
using SportsStore.Core.Domain.DTOs;

namespace SportsStore.Core.CQRS.Queries;

public record GetOrdersByStatusQuery(
    string Status,
    int Page = 1,
    int PageSize = 20) : IRequest<PaginatedOrdersResponse>;
