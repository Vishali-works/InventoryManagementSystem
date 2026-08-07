using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly AppDbContext _context;

    public PurchasesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchases()
    {
        return await _context.Purchases.Include(p => p.Product).Include(p => p.Supplier).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Purchase>> GetPurchase(int id)
    {
        var purchase = await _context.Purchases.Include(p => p.Product).Include(p => p.Supplier).FirstOrDefaultAsync(p => p.PurchaseId == id);
        if (purchase is null)
        {
            return NotFound();
        }

        return purchase;
    }

    [HttpPost]
    public async Task<ActionResult<Purchase>> PostPurchase(Purchase purchase)
    {
        if (purchase.Quantity <= 0 || purchase.UnitPrice < 0)
        {
            return BadRequest("Quantity must be greater than zero and price cannot be negative.");
        }

        var product = await _context.Products.FindAsync(purchase.ProductId);
        if (product is null)
        {
            return BadRequest("Invalid product.");
        }

        purchase.TotalAmount = purchase.Quantity * purchase.UnitPrice;
        _context.Purchases.Add(purchase);
        product.QuantityInStock += purchase.Quantity;
        await _context.SaveChangesAsync();

        /*_context.Purchases.Add(purchase);
        product.QuantityInStock += purchase.Quantity;
        await _context.SaveChangesAsync();*/

        return CreatedAtAction(nameof(GetPurchase), new { id = purchase.PurchaseId }, purchase);
    }
}
