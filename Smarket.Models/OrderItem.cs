namespace Smarket.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int PackageId { get; set; }
        public Package Package { get; set; }

    }
}
