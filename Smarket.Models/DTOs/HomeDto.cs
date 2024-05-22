using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smarket.Models.DTOs
{
    public class HomeDto
    {
        public IEnumerable<CategoryDtoImageUrl> Categories { get; set; }
        public IEnumerable<FlatProductDto> Products { get; set; }
    }
}
