namespace Smarket.Models.DTOs
{
    public class ShoppingCartDto
    {
        public string Username { get; set; }
        public List<PackageDtoForCart> Packages { get; set; }

    }
}
