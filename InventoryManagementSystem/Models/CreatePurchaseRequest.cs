namespace InventoryManagementSystem.Models
{
    public class CreatePurchaseRequest
    {
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }

    }
}
