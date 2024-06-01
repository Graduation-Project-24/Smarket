using Microsoft.AspNetCore.Http;

namespace Smarket.Models.DTOs
{
    public class ImageUploadDto
    {
       public IFormFile formFile { get; set; } = null;
    }
}
