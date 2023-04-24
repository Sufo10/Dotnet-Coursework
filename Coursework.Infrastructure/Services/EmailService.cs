using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
//using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
//using MimeKit;
using Microsoft.Extensions.Configuration;
using Coursework.Domain.Models;
using System.Xml.Linq;

namespace Coursework.Infrastructure.Services
{
    public class EmailService: IEmailService
    {
        private readonly string _from;
        private readonly SmtpClient _client;
        private readonly string _applicationUrl;
        public EmailService(IConfiguration configuration) {
            var userName = configuration.GetSection("EmailConfig:UserName").Value!;
            var password = configuration.GetSection("EmailConfig:Password").Value!;
            _from = userName;
            _client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(userName, password),
                UseDefaultCredentials = false,
                EnableSsl = true
             };
            _applicationUrl = configuration.GetSection("BaseUrl:Frontend").Value!;
        }

        public async Task SendEmailAsync(MessageModel message)
        {
            var mailMessage = new MailMessage(_from, message.To, message.Subject, message.Body)
            {
                IsBodyHtml = true,
            };
            //foreach (var attachment in message.AttachmentPaths.Select(a => new Attachment(a)))
            //{
            //    mailMessage.Attachments.Add(attachment);
            //    }
            await _client.SendMailAsync(mailMessage);
            }

        public async Task SendForgotPasswordEmailAsync(string name, string toEmail, string passwordResetToken)
        {
            var passwordRestUrl = $"{_applicationUrl}reset-password?token={passwordResetToken}&email={toEmail}";
            var message = new MessageModel
            {
                Subject = "Password Reset Request",
                To = toEmail,
                Body = @$"Dear {name}, To reset your password, please click on the following link: <a href='{passwordRestUrl}'>{passwordRestUrl}</a>"
            };
            await SendEmailAsync(message);
        }

        public async Task SendEmailConfirmationAsync(string name, string userId, string toEmail, string token)
        {

            var confirmationUrl = $"https://localhost:7190/api/confirm-email?userId={userId}&token={token}";
            var message = new MessageModel
            {
                Subject = "Confirm your email",
                To = toEmail,
                Body = @$"Dear {name}, Thank you for registering. To confirm your email address, please click on the following link: <a href='{confirmationUrl}'>{confirmationUrl}</a>"
            };
            await SendEmailAsync(message);
        }
    }
}
