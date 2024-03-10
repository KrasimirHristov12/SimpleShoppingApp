using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Configuration;

namespace SimpleShoppingApp.Services.Emails
{
    public class EmailsService : IEmailsService
    {
        private readonly IConfiguration config;

        public EmailsService(IConfiguration _config)
        {
            config = _config;
        }
        public async Task<bool> SendAsync(string toEmail, string toName, string subject, string content)
        {
            var apiKey = config["SendGrid:ApiKey"];
            var fromEmail = config["SendGrid:FromEmail"];
            var fromName = config["SendGrid:FromName"];
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(fromEmail, fromName),
                Subject = subject,
                PlainTextContent = content,
            };
            msg.AddTo(new EmailAddress(toEmail, toName));
            var response = await client.SendEmailAsync(msg); 
            return response.IsSuccessStatusCode;
        }
    }
}
