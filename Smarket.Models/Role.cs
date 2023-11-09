namespace Smarket.Models
{
	public class Role : IdentityRole<int>
    {
        public ICollection<UserRole> UsersRole { get; set; }
    }
}