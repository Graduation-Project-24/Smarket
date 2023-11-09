namespace Smarket.Models
{
    public class Package
    {
        public int Id { get; set; }
        public double ListPrice { get; set; }
        public DateTime Date { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Stock { get; set; }
        public double Price { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public int CartItemId { get; set; }
        public CartItem CartItem { get; set; }
        public int OrderItemId { get; set; }
        public OrderItem OrderItem { get; set; }

    }
}
