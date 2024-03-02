using Microsoft.AspNetCore.Http;

using System.ComponentModel.DataAnnotations;

namespace Smarket.Models.ViewModels
{
    public class ProductDtoAllProductAxies
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int XAxies { get; set; }
        public int YAxies { get; set; }



    }
}
