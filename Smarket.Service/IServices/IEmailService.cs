using Smarket.Models;


namespace Smarket.Services.IServices
{
    public interface IEmailService
    {
        Task EmailSender(string email, string subject, string htmlMessage);
    }
}