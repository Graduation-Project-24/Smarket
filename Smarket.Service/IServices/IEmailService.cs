using Smarket.Models;


namespace Smarket.Services.IServices
{
    public interface IEmailService
    {
        public Task EmailSender(string email, string subject, string htmlMessage);
    }
}