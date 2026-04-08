using MediatR;
using SportsStore.Core.Domain.DTOs;

namespace SportsStore.Core.CQRS.Queries;

public record GetOrderByIdQuery(int OrderId) : IRequest<OrderResponseDto?>;
