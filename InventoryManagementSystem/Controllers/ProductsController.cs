using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return await _context.Products.Include(p => p.Supplier).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.Include(p => p.Supplier).FirstOrDefaultAsync(p => p.ProductId == id);
        if (product is null)
        {
            return NotFound();
        }

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.ProductName))
        {
            return BadRequest("Product name is required.");
        }

        if (product.Price < 0)
        {
            return BadRequest("Price cannot be negative.");
        }

        if (product.QuantityInStock < 0 || product.ReorderLevel < 0)
        {
            return BadRequest("Quantity values cannot be negative.");
        }

        if (product.SupplierId <= 0)
        {
            product.SupplierId = await GetOrCreateDefaultSupplierIdAsync();
        }
        else
        {
            var supplier = await _context.Suppliers.FindAsync(product.SupplierId);
            if (supplier is null)
            {
                product.SupplierId = await GetOrCreateDefaultSupplierIdAsync();
            }
        }

        try
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Unable to save product. {ex.InnerException?.Message ?? ex.Message}");
        }

        return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
    }

    private async Task<int> GetOrCreateDefaultSupplierIdAsync()
    {
        var existingSupplier = await _context.Suppliers.FirstOrDefaultAsync();
        if (existingSupplier is not null)
        {
            return existingSupplier.SupplierId;
        }

        var defaultSupplier = new Supplier
        {
            SupplierName = "Default Supplier",
            ContactPerson = "System",
            Phone = "0000000000",
            Email = "default@supplier.com",
            Address = "N/A"
        };

        _context.Suppliers.Add(defaultSupplier);
        await _context.SaveChangesAsync();
        return defaultSupplier.SupplierId;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, Product product)
    {
        if (id != product.ProductId)
        {
            return BadRequest();
        }

        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
