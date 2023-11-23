using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Smarket.Models.Dtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public double Rate { get; set; }
        public string Comment { get; set; }
        public int ProductId { get; set; }

        [ValidateNever]
        public Product Product { get; set; }
        public int UserId { get; set; }

        [ValidateNever]
        public User User { get; set; }
    }
}
