namespace SimpleShoppingApp.Services.Emails
{
    public interface IEmailsService
    {
        Task<bool> SendAsync(string toEmail, string toName, string subject, string content);
    }
}
