using InventoryManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ReportsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<object>>> GetLowStock()
    {
        var result = await _context.Products
            .Where(p => p.QuantityInStock < p.ReorderLevel)
            .Select(p => new { p.ProductId, p.ProductName, p.QuantityInStock, p.ReorderLevel })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("stock-summary")]
    public async Task<ActionResult<object>> GetStockSummary()
    {
        var summary = await _context.Products
            .GroupBy(p => p.Category ?? "Uncategorized")
            .Select(g => new { Category = g.Key, TotalQuantity = g.Sum(p => p.QuantityInStock), TotalValue = g.Sum(p => p.Price * p.QuantityInStock) })
            .ToListAsync();

        return Ok(summary);
    }
}
