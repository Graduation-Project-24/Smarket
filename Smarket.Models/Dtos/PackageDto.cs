
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Smarket.Models.ViewModels
{
    public class PackageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public double ListPrice { get; set; } = 00;
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
        public DateTime ExpireDate { get; set; }
        public int Stock { get; set; } = 0;

        [Range(1, 1000)]
        public double Price { get; set; } = 00;
        public int ProductId { get; set; }

        [ValidateNever]
        public Product Product { get; set; }
        public int InventoryId { get; set; }

        [ValidateNever]
        public Inventory Inventory { get; set; }
        public int CartItemId { get; set; }

        [ValidateNever]
        public CartItem CartItem { get; set; }
        public int OrderItemId { get; set; }

        [ValidateNever]
        public OrderItem OrderItem { get; set; }

        [ValidateNever]
        public IEnumerable<Inventory> Inventories { get; set; }

        [ValidateNever]
        public IEnumerable<Product> Products { get; set; }

    }
}
