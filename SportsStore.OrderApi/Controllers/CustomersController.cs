using MediatR;
using Microsoft.AspNetCore.Mvc;
using SportsStore.Core.CQRS.Queries;

namespace SportsStore.OrderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{customerId}/orders")]
    public async Task<IActionResult> GetCustomerOrders(string customerId)
    {
        var result = await _mediator.Send(new GetCustomerOrdersQuery(customerId));
        return Ok(result);
    }
}
