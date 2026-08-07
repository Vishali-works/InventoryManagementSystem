using InventoryManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

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

    // GET: api/reports/low-stock — uses CTE + Window Function stored procedure

    [HttpGet("low-stock")]

    public IActionResult GetLowStock()

    {

        List<object> lowStockItems = new();

        string connectionString = _context.Database.GetConnectionString();



        using SqlConnection conn = new(connectionString);

        conn.Open();



        using SqlCommand cmd = new("usp_GetLowStockReport", conn);

        cmd.CommandType = System.Data.CommandType.StoredProcedure;



        using SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())

        {

            lowStockItems.Add(new

            {

                ProductId = reader["ProductId"],

                ProductName = reader["ProductName"],

                Category = reader["Category"],

                QuantityInStock = reader["QuantityInStock"],

                ReorderLevel = reader["ReorderLevel"],

                ShortfallQty = reader["ShortfallQty"],

                SupplierName = reader["SupplierName"],

                SupplierPhone = reader["SupplierPhone"],

                UrgencyRank = reader["UrgencyRank"]

            });

        }

        return Ok(lowStockItems);

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
