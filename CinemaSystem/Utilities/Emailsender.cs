using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace CinemaSystem.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("zakariaadham650@gmail.com", "hhxy hihm nwtd jltt")
            };
            return client.SendMailAsync(

                new MailMessage(
                    from: "zakariaadham650@gmail.com",
                    to:email,
                    subject,
                    htmlMessage
                    )
                {
                    IsBodyHtml = true
                }); 
        }
    }
}
