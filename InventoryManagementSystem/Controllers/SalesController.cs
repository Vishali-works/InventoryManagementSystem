using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SalesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sale>>> GetSales()
    {
        return await _context.Sales.Include(s => s.Product).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sale>> GetSale(int id)
    {
        var sale = await _context.Sales.Include(s => s.Product).FirstOrDefaultAsync(s => s.SalesId == id);
        if (sale is null)
        {
            return NotFound();
        }

        return sale;
    }

    [HttpPost]
    public async Task<ActionResult<Sale>> PostSale(Sale sale)
    {
        if (sale.Quantity <= 0 || sale.UnitPrice < 0)
        {
            return BadRequest("Quantity must be greater than zero and price cannot be negative.");
        }

        var product = await _context.Products.FindAsync(sale.ProductId);
        if (product is null)
        {
            return BadRequest("Invalid product.");
        }

        if (product.QuantityInStock < sale.Quantity)
        {
            return BadRequest("Not enough stock available.");
        }


        sale.TotalAmount = sale.Quantity * sale.UnitPrice;
        _context.Sales.Add(sale);
        product.QuantityInStock -= sale.Quantity;
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSale), new { id = sale.SalesId }, sale);
    }
}
