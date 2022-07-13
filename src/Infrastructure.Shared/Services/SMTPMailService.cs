using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Application.Interfaces.Services;
using EDO_FOMS.Application.Requests.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace EDO_FOMS.Infrastructure.Shared.Services
{
    public class SMTPMailService : IMailService
    {
        private readonly MailConfiguration _mail;
        private readonly ILogger<SMTPMailService> _logger;

        public SMTPMailService(IOptions<MailConfiguration> mailConfig, ILogger<SMTPMailService> logger)
        {
            _mail = mailConfig.Value;
            _logger = logger;
        }

        public async Task SendAsync(MailRequest request)
        {
            try
            {
                var email = new MimeMessage();
                //{
                    //Sender = new MailboxAddress(_config.DisplayName, request.From ?? _config.From),
                    //Subject = request.Subject,
                    //Body = new BodyBuilder {HtmlBody = request.Body}.ToMessageBody()
                //};

                email.From.Add(new MailboxAddress(_mail.DisplayName, _mail.From));
                email.To.Add(new MailboxAddress(string.IsNullOrWhiteSpace(request.ToName) ? "EDO User" : request.ToName, request.ToAddress));
                email.Subject = request.Subject;
                email.Body = new BodyBuilder { HtmlBody = request.Body }.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_mail.Host, _mail.Port, SecureSocketOptions.Auto);
                
                if (!string.IsNullOrWhiteSpace(_mail.Password))
                {
                    await smtp.AuthenticateAsync(_mail.UserName, _mail.Password);
                }
                
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error:MailSendError - Type: {Type} Message: {Message}", ex.GetType(), ex.Message);
            }
        }
    }
}