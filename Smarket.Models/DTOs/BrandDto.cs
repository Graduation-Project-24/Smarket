namespace Smarket.Models.DTOs
{
    public class BrandDto
    {
        public string Name { get; set; }
        public int ImageId { get; set; }
        public IEnumerable<Brand> Brands { get; set; }

    }
}
