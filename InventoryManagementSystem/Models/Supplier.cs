using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryManagementSystem.Models;

public class Supplier
{
    public int SupplierId { get; set; }

    [Required]
    [StringLength(100)]
    public string SupplierName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? ContactPerson { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(250)]
    public string? Address { get; set; }

    [JsonIgnore]
    public ICollection<Product> Products { get; set; } = new List<Product>();
    [JsonIgnore]
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
