using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryManagementSystem.Models;

public class Sale
{
    [Key]
    public int SalesId { get; set; }

    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public DateTime SalesDate { get; set; } = DateTime.UtcNow;

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    public decimal TotalAmount { get; set; }

    [JsonIgnore]
    public Product? Product { get; set; }
}
