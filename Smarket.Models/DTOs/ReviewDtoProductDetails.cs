using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Smarket.Models.Dtos
{
    public class ReviewDtoProductDetails
    {
        public double Rate { get; set; }
        public string Comment { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
    }
}
