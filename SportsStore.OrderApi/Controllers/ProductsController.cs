using Microsoft.AspNetCore.Mvc;
using SportsStore.Core.Domain.Interfaces;

namespace SportsStore.OrderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IStoreRepository _storeRepository;

    public ProductsController(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? category = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var products = await _storeRepository.GetProductsAsync(page, pageSize, category);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _storeRepository.GetProductByIdAsync(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var products = _storeRepository.Products;
        var categories = products.Select(p => p.Category).Distinct().ToList();
        return Ok(categories);
    }
}
