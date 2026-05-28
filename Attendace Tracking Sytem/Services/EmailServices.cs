using System.Threading.Tasks;
using Attendace_Tracking_Sytem.ApiSettings;
using Attendace_Tracking_Sytem.Interface;
using Microsoft.Extensions.Options;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

namespace Attendace_Tracking_Sytem.Services
{
    public class EmailServices
    {
        private BrevoSettings _settings;

        public EmailServices(IOptions<BrevoSettings>options)
        {
            _settings = options.Value;
        }

        public async void sendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            //SET yung api key para sa brevo
            Configuration.Default.ApiKey["api-key"] = _settings.apiKey;

           //INTIATE EMAILS API
            var apiInstance = new TransactionalEmailsApi();

            //SET UP SENDER 
            var sender = new SendSmtpEmailSender
            {
                Name = _settings.senderName,
                Email = _settings.senderEmail
            };

            //SET UP RECEIVER
            var receiver = new List<SendSmtpEmailTo>()
            {
                new SendSmtpEmailTo(toEmail)
            };

            //generate the email
            var email = new SendSmtpEmail(
               sender: sender,
               to: receiver,
               subject: subject,
               htmlContent: htmlContent
            );

            //send the email
            await apiInstance.SendTransacEmailAsync(email);
        }
    }
}
