using Google.Apis.Auth.OAuth2;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace intStore.Models
{
    public class EmailService
    {
        //private const string ClientId = "your-client-id";
        //private const string ClientSecret = "your-client-secret";
        //private const string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var credential = await GetGoogleCredentialAsync();
            var accessToken = await credential.GetAccessTokenForRequestAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Max", "ultramax765@gmail.com"));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("ultramax765@gmail.com", accessToken);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            Console.WriteLine("Email sent successfully!");
        }

        private async Task<UserCredential> GetGoogleCredentialAsync()
        {
            using (var stream = new FileStream("path-to-your-client-secret.json", FileMode.Open, FileAccess.Read))
            {
                return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { "https://mail.google.com/" },
                    "user",
                    CancellationToken.None
                );
            }
        }
    }
}
