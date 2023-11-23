namespace Smarket.Models
{
    public class Package
    {
        public int Id { get; set; }
        public double ListPrice { get; set; }
        public DateTime Date { get; set; }
        public DateTime ExpireDate { get; set; }
        public int Stock { get; set; }
        public int left { get; set; }
        public double Price { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public ICollection<OrderItem> Items { get; set;}
        public ICollection<CartItem> Carts { get; set;}

    }
}
