namespace Smarket.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ImageId { get; set; }
        public  Image Image { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
