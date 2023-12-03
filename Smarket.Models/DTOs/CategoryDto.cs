using Microsoft.AspNetCore.Http;

namespace Smarket.Models.DTOs
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public IFormFile formFile { get; set; }
    }
}
