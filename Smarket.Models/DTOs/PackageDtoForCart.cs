namespace Smarket.Models.DTOs
{
    public class PackageDtoForCart
    {
        public int PackageId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; } = 00;
        public double ListPrice { get; set; } = 00;
        public int Count { get; set; }
        public string ProductImageUrl { get; set; }

    }
}
