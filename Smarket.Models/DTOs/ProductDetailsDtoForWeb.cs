using Smarket.Models.Dtos;

namespace Smarket.Models.DTOs
{
    public class ProductDetailsDtoForWeb
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Left { get; set; }
        public string SubCategoryName { get; set; }
        public string ImageUrl { get; set; }
        public string BrandName { get; set; }
        public double price { get; set; }
        public int BrandId { get; set; }

        public IEnumerable<ProductDtoFilter> productDtoFilters { get; set; }
        public List<ReviewDtoProductDetails> Reviews { get; set; }

        public double AverageRate
        {
            get
            {
                if (Reviews == null || Reviews.Count == 0)
                    return 0;

                return Reviews.Average(r => r.Rate);
            }
        }

    }
}
