namespace Smarket.Models.Dtos
{
    public class RegisterDto
    {
        public string UsertName { get; set; }
        public int ImageId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
