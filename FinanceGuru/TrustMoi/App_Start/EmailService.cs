using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace TrustMoi
{
    public class EmailService : IIdentityMessageService
    {
        private const string EmailHost = "smtp.gmail.com";
        private const int EmailPort = 587;
        private const string CredentialUserName = "trustmoi.dev@gmail.com";
        private const string CredentialPassword = "1LakhRupees!";
        private const string SentFromEmail = "trustmoi.dev@gmail.com";
        private const string SentFromName = "TrustMoi Dev";

        public async Task SendAsync(IdentityMessage message)
        {
            var email = new MailMessage(new MailAddress(SentFromEmail, SentFromName), new MailAddress(message.Destination))
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };

            using (var client = GetSmtpClient())
            {
                await client.SendMailAsync(email);
            }
        }

        private static SmtpClient GetSmtpClient()
        {
            var credentials = new System.Net.NetworkCredential(CredentialUserName, CredentialPassword);
            var client = new SmtpClient(EmailHost, EmailPort)
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = credentials
                };

            return client;
        }
    }
}
