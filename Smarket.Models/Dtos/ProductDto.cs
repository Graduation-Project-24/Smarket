using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace Smarket.Models.ViewModels
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(0, 5)]
        public int Rate { get; set; }
        public double? AvgRate { get; set; }

        public string Comment { get; set; }
        public int BrandId { get; set; }

        public int SubCategoryId { get; set; }

        public IFormFile formFile { get; set; }


    }
}
