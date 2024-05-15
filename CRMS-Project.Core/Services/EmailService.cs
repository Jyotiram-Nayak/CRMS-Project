using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.DTO.Email;
using CRMS_Project.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;

namespace CRMS_Project.Core.Services
{
    public class EmailService : IEmailService
    {
        private const string templatePath = @"wwwroot/EmailTemplate/{0}.html";
        private readonly IOptions<SMTPConfiguration> _smtpconfig;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public EmailService(IOptions<SMTPConfiguration> smtpconfig,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _smtpconfig = smtpconfig;
            _configuration = configuration;
            _userManager = userManager;
            _environment = environment;
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
                string appDomain = _configuration.GetSection("Application:AppDomain").Value ?? "";
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
            MailMessage mailMessage = new MailMessage
            {
                Subject = emailMessage.Subject,
                Body = emailMessage.Body,
                From = new MailAddress("no-replay@LibraryManagement.com", "Head of the department"),
                IsBodyHtml = true
                //From = new MailAddress(_configuration.GetSection("EmailSettings.SenderEmail").Value ?? "", _configuration.GetSection("EmailSettings.SenderName").Value ?? ""),
                //IsBodyHtml = Convert.ToBoolean(_configuration.GetSection("EmailSettings.IsBodyHTML").Value),
            };
            //// for multiple email sending
            foreach (var toEmail in emailMessage.ToEmails)
            {
                mailMessage.To.Add(toEmail);
            }
            //mailMessage.To.Add(emailMessage.ToEmails);
            NetworkCredential networkCredential = new NetworkCredential("2f20c76a19b259", "1898ebb259012c");
            //NetworkCredential networkCredential = new NetworkCredential(_configuration.GetSection("EmailSettings.SmtpUsername").Value, _configuration.GetSection("EmailSettings.SmtpPassword").Value);

            SmtpClient smtpClient = new SmtpClient
            {
                Host = "sandbox.smtp.mailtrap.io",
                Port = 587,
                EnableSsl = true,
                Credentials = networkCredential
                //Host = _configuration.GetSection("EmailSettings.SmtpServer").Value ?? "",
                //Port = Convert.ToInt32(_configuration.GetSection("EmailSettings.SmtpPort").Value),
                //EnableSsl = Convert.ToBoolean(_configuration.GetSection("EmailSettings.SmtpPort").Value),
                //Credentials = networkCredential
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
