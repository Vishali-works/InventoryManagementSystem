using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryManagementSystem.Models;

public class Product
{
    public int ProductId { get; set; }

    [Required]
    [StringLength(100)]
    public string ProductName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Category { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int QuantityInStock { get; set; }

    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; }

    public int SupplierId { get; set; }

    public Supplier? Supplier { get; set; }
    [JsonIgnore]
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    [JsonIgnore]
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
