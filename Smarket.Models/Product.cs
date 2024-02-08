using System.Diagnostics.CodeAnalysis;

namespace Smarket.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<UserReview> Reviews { get; set; }
        public int? BrandId { get; set; }
        public Brand Brand { get; set; }

        public int? SubCategoryId { get; set; } 
        public SubCategory SubCategory { get; set; } 
        public int ImageId { get; set; } = 0;

        [AllowNull]
        public Image Image { get; set; } 

    }
}
