namespace Smarket.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ImageId { get; set; }
        public Image Image { get; set; }
    }
}
