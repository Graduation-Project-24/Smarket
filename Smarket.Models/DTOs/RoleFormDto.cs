using System.ComponentModel.DataAnnotations;

namespace Smarket.Models.DTOs
{
    public class RoleFormDto
    {
        [Required, StringLength(256)]
        public string Name { get; set; }

    }
}
