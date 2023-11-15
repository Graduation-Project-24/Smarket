using Smarket.Services.IServices;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace Smarket.Services
{
    public class EmailService : IEmailService
    {
        [AllowAnonymous]
        public async Task EmailSender(string email, string subject, string htmlMessage)
        {
            var fromMail = "graduationp629@outlook.com";
            var fromPassword = "AsdAsd123456";

            var message = new MailMessage();

            message.From = new MailAddress(fromMail);
            message.To.Add(email);
            message.Body = $"<html><body> {htmlMessage}</body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp-mail.outlook.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);
        }
    }
}