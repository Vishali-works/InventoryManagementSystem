using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryManagementSystem.Models;

public class Purchase
{
    public int PurchaseId { get; set; }

    public int ProductId { get; set; }
    public int SupplierId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    public decimal TotalAmount { get; set; }

    [JsonIgnore]
    public Product? Product { get; set; }
    [JsonIgnore]
    public Supplier? Supplier { get; set; }
}
