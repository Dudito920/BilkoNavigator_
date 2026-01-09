using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace BilkoNavigator_.Models
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Placeholder implementation for development
            return Task.CompletedTask;
        }
    }
}