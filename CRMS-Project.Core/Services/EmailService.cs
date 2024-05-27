using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.DTO.Email;
using CRMS_Project.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System.Text;
using CRMS_Project.Core.DTO.Request;

namespace CRMS_Project.Core.Services
{
    public class EmailService : IEmailService
    {
        private const string templatePath = @"wwwroot/EmailTemplate/{0}.html";
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        public EmailService(IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }
        public async Task SendEmailConfirmationAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                //await SendEmailConfirmationAsync(user, token);
                string appDomain = _configuration.GetSection("Application:AppDomain").Value ?? "";
                string confirmLink = _configuration.GetSection("Application:EmailConfirmation").Value ?? "";
                EmailMessage emailMessage = new EmailMessage
                {
                    ToEmails = new List<string>() { user.Email },
                    PlaceHolders = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("{{UserName}}",user.Email),
                        new KeyValuePair<string, string>("{{Link}}",string.Format(appDomain+confirmLink,user.Id,token))
                    }
                };
                emailMessage.Subject = UpdatePlaceHolders("Hellow {{UserName}}! Confirm Your email", emailMessage.PlaceHolders);
                emailMessage.Body = UpdatePlaceHolders(GetEmailBody("EmailConfirmation"), emailMessage.PlaceHolders);
                await SendEmailAsync(emailMessage);
            }
        }
        public async Task SendForgotEmailAsync(ApplicationUser user)
        {
            //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                //await SendEmailConfirmationAsync(user, token);
                string appDomain = _configuration.GetSection("Application:frontendDomain").Value ?? "";
                string confirmLink = _configuration.GetSection("Application:ForgotPassword").Value ?? "";
                EmailMessage emailMessage = new EmailMessage
                {
                    ToEmails = new List<string>() { user.Email },
                    PlaceHolders = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("{{UserName}}",user.Email),
                        new KeyValuePair<string, string>("{{Link}}",string.Format(appDomain+confirmLink,user.Id,token))
                    }
                };
                emailMessage.Subject = UpdatePlaceHolders("Hellow {{UserName}}! reset your password", emailMessage.PlaceHolders);
                emailMessage.Body = UpdatePlaceHolders(GetEmailBody("ForgotPassword"), emailMessage.PlaceHolders);
                await SendEmailAsync(emailMessage);
            }
        }

        public async Task SendContactusEmailAsync(ApplicationUser user,ContactUsRequest contact)
        {

            EmailMessage emailMessage = new EmailMessage
            {
                ToEmails = new List<string>() { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("{{UserName}}",user.Email),
                        new KeyValuePair<string, string>("{{Name}}",contact.FirstName+" "+contact.LastName),
                        new KeyValuePair<string, string>("{{Email}}",contact.Email),
                        new KeyValuePair<string, string>("{{Message}}",contact.Message),
                    }
            };
            emailMessage.Subject = UpdatePlaceHolders("Hellow {{UserName}}! someone try to reach you", emailMessage.PlaceHolders);
            emailMessage.Body = UpdatePlaceHolders(GetEmailBody("ContactUs"), emailMessage.PlaceHolders);
            await SendEmailAsync(emailMessage);

        }
        /// <summary>
        /// Get email body from templates.
        /// </summary>
        /// <param name="tempemailName"></param>
        /// <returns> read the body from template </returns>
        private string GetEmailBody(string tempemailName)
        {
            var body = File.ReadAllText(string.Format(templatePath, tempemailName));
            return body;
        }
        /// <summary>
        /// after registration sending email confirmation message 
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        private async Task SendEmailAsync(EmailMessage emailMessage)
        {
            var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "no-replay@LibraryManagement.com";
            var senderName = _configuration["EmailSettings:SenderName"];
            var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var enableSSL = bool.Parse(_configuration["EmailSettings:EnableSSL"]);
            var isBodyHTML = bool.Parse(_configuration["EmailSettings:IsBodyHTML"]);
            MailMessage mailMessage = new MailMessage
            {
                Subject = emailMessage.Subject,
                Body = emailMessage.Body,
                //From = new MailAddress("no-replay@LibraryManagement.com", "Head of the department"),
                From = new MailAddress(senderEmail, senderName),
                IsBodyHtml = isBodyHTML
            };
            //// for multiple email sending
            foreach (var toEmail in emailMessage.ToEmails)
            {
                mailMessage.To.Add(toEmail);
            }
            //NetworkCredential networkCredential = new NetworkCredential("818b9c67acbd08", "f9ba442d2111c5");
            NetworkCredential networkCredential = new NetworkCredential(smtpUsername,smtpPassword );

            SmtpClient smtpClient = new SmtpClient
            {
                //Host = "sandbox.smtp.mailtrap.io",
                //Port = 587,
                //EnableSsl = true,
                //Credentials = networkCredential
                Host = smtpServer,
                Port = smtpPort,
                EnableSsl = enableSSL,
                Credentials = networkCredential
            };
            try
            {
                mailMessage.BodyEncoding = Encoding.Default;
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Update the Email body
        /// </summary>
        /// <param name="text"></param>
        /// <param name="keyValuePairs"></param>
        /// <returns> return the email body after modification </returns>
        private string UpdatePlaceHolders(string text, List<KeyValuePair<string, string>> keyValuePairs)
        {
            if (!string.IsNullOrEmpty(text) && keyValuePairs != null)
            {
                foreach (var PlaceHolder in keyValuePairs)
                {
                    if (text.Contains(PlaceHolder.Key))
                    {
                        text = text.Replace(PlaceHolder.Key, PlaceHolder.Value);
                    }
                }
            }
            return text;
        }
    }
}
