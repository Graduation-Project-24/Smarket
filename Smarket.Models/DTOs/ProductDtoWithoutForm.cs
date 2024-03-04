using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace Smarket.Models.ViewModels
{
    public class ProductDtoWithoutForm
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int BrandId { get; set; }
        public int SubCategoryId { get; set; }
        public int XAxies { get; set; }
        public int YAxies { get; set; }
        public string ImageUrl { get; set; }



    }
}
