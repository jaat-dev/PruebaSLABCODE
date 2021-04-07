using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Projects.Api.Models;
using Projects.Api.Models.Responses;
using Projects.Api.Options;
using System;

namespace Projects.Api.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IOptions<MailSettings> _mailSettings;

        public MailHelper(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public Response SendMail(EmailMessageModel emailMessage)
        {
            try
            {
                string from = _mailSettings.Value.From;
                string smtp = _mailSettings.Value.Smtp;
                string pass = _mailSettings.Value.Password;
                int port = _mailSettings.Value.Port;
                MimeMessage message = new();
                message.From.Add(new MailboxAddress("From", from));
                message.To.Add(new MailboxAddress("To", emailMessage.To));
                message.Subject = emailMessage.Subject;
                BodyBuilder bodyBuilder = new()
                {
                    HtmlBody = emailMessage.Content
                };
                message.Body = bodyBuilder.ToMessageBody();

                using (SmtpClient client = new())
                {
                    client.Connect(smtp, port, false);
                    client.Authenticate(from, pass);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return new Response { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Result = ex
                };
            }
        }
    }
}
