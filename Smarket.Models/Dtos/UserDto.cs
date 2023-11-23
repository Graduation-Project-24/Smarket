namespace Smarket.Models.Dtos
{
    public class UserDto
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Token { get; set; }
    }
}
