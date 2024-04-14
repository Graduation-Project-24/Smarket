using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smarket.Models.DTOs
{
    public class PackageDtoForCart
    {
        public int PackageId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; } = 00;
        public double ListPrice { get; set; } = 00;
        public string ProductImageUrl { get; set; }

    }
}
