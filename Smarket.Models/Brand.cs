namespace Smarket.Models
{
    public class Brand
    {          
        public int Id { get; set; }
        public string Name { get; set; }
        public int ImageId { get; set; }
        public Image Image { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
