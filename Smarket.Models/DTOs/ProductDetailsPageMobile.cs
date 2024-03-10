using Smarket.Models.Dtos;

namespace Smarket.Models.DTOs
{
    public class ProductDetailsPageMobile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SubCategoryName { get; set; }
        public string ImageUrl { get; set; }
        public string BrandName { get; set; }
        public double price { get; set; }
        public int BrandId { get; set; }
        public int SubCategoryId { get; set; }

        public IEnumerable<ProductDtoFilter> productDtoFilters { get; set; }
        public List<ReviewDtoProductDetails> Reviews { get; set; }
    }
}
