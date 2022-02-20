using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ZO1.Identity.WebApp.Settings;

namespace ZO1.Identity.WebApp.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IOptions<SmtpSetting> _smtpSetting;

        public EmailServices(IOptions<SmtpSetting> smtpSetting)
        {
            _smtpSetting = smtpSetting;
        }
        public async Task SendAsync(string from, string to, string subject, string body)
        {
            var message = new MailMessage(from, to, subject, body);

            // use http client to send email
            using var emailClient = new SmtpClient(_smtpSetting.Value.Host, _smtpSetting.Value.Port)
            {
                Credentials = new NetworkCredential(
                    _smtpSetting.Value.Email,
                    _smtpSetting.Value.Password
                ),
                EnableSsl = true
            };

            await emailClient.SendMailAsync(message);
        }
    }
}