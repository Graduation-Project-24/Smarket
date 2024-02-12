using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace Smarket.Models.ViewModels
{
    public class ProductDtoWithCBNames
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SubCategoryName { get; set; }
        public string ImageUrl { get; set; }
        public string BrandName { get; set; }
        public int BrandId { get; set; }
        public int SubCategoryId { get; set; }


    }
}
