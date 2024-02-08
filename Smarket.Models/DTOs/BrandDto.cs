using Microsoft.AspNetCore.Http;

namespace Smarket.Models.DTOs
{
    public class BrandDto
    {
        public string Name { get; set; }
        public IFormFile formFile { get; set; }
        public string ImageUrl { get; set; }

    }
}
