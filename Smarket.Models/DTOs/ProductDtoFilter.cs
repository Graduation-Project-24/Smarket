using Microsoft.AspNetCore.Http;

namespace Smarket.Models.DTOs
{
    public class ProductDtoFilter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SubCategoryName { get; set; }
        public string ImageUrl { get; set; }
        public string BrandName { get; set; }

    }
}
