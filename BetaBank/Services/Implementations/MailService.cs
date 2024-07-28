using BetaBank.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BetaBank.Services.Implementations
{
    public class MailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_configuration["SecondaryMailSettings:Mail"]);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_configuration["SecondaryMailSettings:Host"], int.Parse(_configuration["SecondaryMailSettings:Port"]), SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration["SecondaryMailSettings:Mail"], _configuration["SecondaryMailSettings:Password"]);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
