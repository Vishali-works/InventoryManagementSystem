namespace InventoryManagementSystem.Models
{
    public class CreateSaleRequest
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
