namespace Smarket.Models.Dtos
{
    public class ResetPasswordDto
    {
        public string token { get; set; }
        public string NewPassword { get; set; }
        public string Email { get; set; }
    }
}
