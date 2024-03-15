using Smarket.Models.Dtos;

namespace Smarket.Models.DTOs
{
    public class ProductDtoWebCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BrandName { get; set; }
        public string SubCategoryName { get; set; }
        public string ImageUrl { get; set; }
        public double Price { get; set; }

        public List<ReviewDtoRateOnly> Reviews { get; set; }

    }
}
