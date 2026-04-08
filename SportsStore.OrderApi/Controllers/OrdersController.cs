using MediatR;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Core.CQRS.Commands;
using SportsStore.Core.CQRS.Queries;

namespace SportsStore.OrderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        var result = await _mediator.Send(new GetOrdersQuery(page, pageSize, status));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetOrderStatus(int id)
    {
        var result = await _mediator.Send(new GetOrderStatusQuery(id));
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id, [FromBody] string? reason = null)
    {
        var result = await _mediator.Send(new CancelOrderCommand(id, reason));
        if (!result)
        {
            return BadRequest("Failed to cancel order");
        }
        return Ok(new { cancelled = true });
    }

    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusCommand command)
    {
        var result = await _mediator.Send(command with { OrderId = id });
        if (!result)
        {
            return BadRequest("Failed to update order status");
        }
        return Ok(new { updated = true });
    }

    [HttpGet("by-status/{status}")]
    public async Task<IActionResult> GetOrdersByStatus(
        string status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetOrdersByStatusQuery(status, page, pageSize));
        return Ok(result);
    }

    [HttpGet("dashboard/summary")]
    public async Task<IActionResult> GetDashboardSummary()
    {
        var result = await _mediator.Send(new GetDashboardSummaryQuery());
        return Ok(result);
    }
}
