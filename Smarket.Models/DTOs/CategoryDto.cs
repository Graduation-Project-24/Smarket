namespace Smarket.Models.DTOs
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public int ImageId { get; set; } = 3;
        public IEnumerable<Category> Categories { get; set; }
    }
}
