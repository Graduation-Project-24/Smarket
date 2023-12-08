
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Smarket.Models.ViewModels
{
    public class PackageDto
    {
        public double ListPrice { get; set; } = 00;
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
        public DateTime ExpireDate { get; set; }
        public int Stock { get; set; } = 0;

        [Range(1, 100000)]
        public double Price { get; set; } = 00;
        public int ProductId { get; set; }

        public int InventoryId { get; set; }

    }
}
