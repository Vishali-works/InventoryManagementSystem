using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Models;

public class User
{
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [StringLength(50)]
    public string Role { get; set; } = "User";
}
