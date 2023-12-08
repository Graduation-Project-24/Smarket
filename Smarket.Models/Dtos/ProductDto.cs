using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace Smarket.Models.ViewModels
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int BrandId { get; set; }
        public int SubCategoryId { get; set; }

        public IFormFile formFile { get; set; }


    }
}
