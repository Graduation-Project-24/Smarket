namespace Smarket.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int PackageId { get; set; }
        public Package Package { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
