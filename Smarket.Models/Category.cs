using System.Diagnostics.CodeAnalysis;

namespace Smarket.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ImageId { get; set; } = 0;
        [AllowNull]
        public Image Image { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; }
    }
}
