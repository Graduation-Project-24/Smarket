namespace Smarket.Models.DTOs
{
    public class FavoriteDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public List<ReviewDtoRateOnly> Reviews { get; set; }

    }
}
