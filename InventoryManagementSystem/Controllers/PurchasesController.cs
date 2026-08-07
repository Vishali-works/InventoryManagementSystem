using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

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

    // POST: api/purchases/create — uses stored procedure with transaction

    [HttpPost("create")]

    public IActionResult CreatePurchaseViaSP([FromBody] CreatePurchaseRequest request)

    {

        string connectionString = _context.Database.GetConnectionString();



        using SqlConnection conn = new(connectionString);

        conn.Open();



        using SqlCommand cmd = new("usp_CreatePurchase", conn);

        cmd.CommandType = System.Data.CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@ProductId", request.ProductId);

        cmd.Parameters.AddWithValue("@SupplierId", request.SupplierId);

        cmd.Parameters.AddWithValue("@Quantity", request.Quantity);

        cmd.Parameters.AddWithValue("@UnitPrice", request.UnitPrice);



        int newId = Convert.ToInt32(cmd.ExecuteScalar());

        return Ok(new { PurchaseId = newId, Message = "Purchase created and stock updated." });

    }
}
