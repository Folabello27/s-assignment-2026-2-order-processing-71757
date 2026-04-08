using MediatR;
using SportsStore.Core.Domain.DTOs;

namespace SportsStore.Core.CQRS.Queries;

public record GetCustomerOrdersQuery(string CustomerId) : IRequest<List<OrderResponseDto>>;
