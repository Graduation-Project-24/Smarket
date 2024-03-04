using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smarket.Models.DTOs
{
    public class PackageDtoWithInfo
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public double Price { get; set; } = 00;
        public double ListPrice { get; set; } = 00;
        public string InventoryName { get; set; }
        public string ProductImageUrl { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
        public DateTime ExpireDate { get; set; }
        public int Stock { get; set; } = 0;
        public int left { get; set; }





    }
}
