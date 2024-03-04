using Microsoft.AspNetCore.Http;

namespace Smarket.Models.DTOs
{
    public class BrandDtoWithImage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public object Image { get; set; }


    }
}
