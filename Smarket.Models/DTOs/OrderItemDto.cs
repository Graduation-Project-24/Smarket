using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smarket.Models.DTOs
{
    public class OrderItemDto
    {
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
    }
}
