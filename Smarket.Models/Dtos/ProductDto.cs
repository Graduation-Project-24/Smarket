
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Smarket.Models.ViewModels
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(1, 1000)]
        public double Price { get; set; } = 00;

        [Range(0, 5)]
        public int Rate { get; set; }
        public double? AvgRate { get; set; }

        public string Comment { get; set; }
        public int BrandId { get; set; }

        [ValidateNever]
        public Brand Brand { get; set; }

        public int SubCategoryId { get; set; }

        [ValidateNever]
        public SubCategory SubCategory { get; set; }
        public int ImageId { get; set; }

        [ValidateNever]
        public Image Image { get; set; }

        [ValidateNever]
        public IEnumerable<Brand> Brands { get; set; }

        [ValidateNever]
        public IEnumerable<SubCategory> SubCategories { get; set; }

        [ValidateNever]
        public IEnumerable<UserReview> UserReviews { get; set; }

    }
}
