using Microsoft.AspNetCore.Http;

namespace Smarket.Models.Dtos
{
    public class EditUserDtoFewData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
/*        public IFormFile formFile { get; set; } = null;*/

    }
}
