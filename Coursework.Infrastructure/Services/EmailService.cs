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
using Newtonsoft.Json.Linq;

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

        public async Task SendPaymentInvoiceAsync(GenerateInvoiceDTO model)
        {
            var message = new MessageModel
            {
                Subject = "Payment Invoice",
                To = model.CustomerEmail,
                Body = GenerateInvoice(model)
            };
            await SendEmailAsync(message);
        }
        public async Task SendEmailAdditionalChargesAsync(string amount, string description, string booking_id, string car_id, string user_id, string created_at_date, string toEmail)
        {
            StringBuilder sb = new();

            sb.Append("<html>");
            sb.Append("<head>");
            sb.Append("<style>");
            sb.Append("table { border-collapse: collapse; width: 100%; }");
            sb.Append("th, td { text-align: left; padding: 8px; border: 1px solid black; }");
            sb.Append("th { background-color: #4CAF50; color: white; }");
            //sb.Append("td { border: 1px solid black; }");
            sb.Append("</style>");
            sb.Append("</head>");

            sb.Append("<body>");
            sb.AppendFormat("<h1>{0}</h1>", "Hajur Ko Car Rental");
            sb.AppendFormat("<p>{0}</p>", @$"Dear Customer, For the damage on the car that you rented. You are required to clear the due amount.");
            sb.AppendFormat("<p>Invoice Date: {0}</p>", DateTime.Now.ToString("yyyy-MM-dd"));
            sb.AppendFormat("<p>Customer Email: {0}</p>", toEmail);

            // sb.AppendFormat("<table><tr><th>Damage Description</th><td>{0}</td></tr><tr><th>Amount</th><td>Rs. {1}</td></tr><tr><th>Damage Repoted Date</th><td>{2}</td></tr></table>", description, amount, created_at_date);
            sb.AppendFormat("<table><tr><th>Booking ID</th><td>{0}</td></tr><tr><th>Car ID</th><td>{1}</td></tr><tr><th>User ID</th><td>{2}</td></tr><tr><th>Damage Description</th><td>{3}</td></tr><tr><th>Amount</th><td>{4}</td></tr><tr><th>Damage Repoted Date</th><td>{5}</td></tr></table>", booking_id, car_id, user_id, description, amount, created_at_date);

            sb.Append("</body>");
            sb.Append("</html>");

            var message = new MessageModel
            {
                Subject = "Damage Invoice",
                To = toEmail,
                Body = sb.ToString(),
            };
            await SendEmailAsync(message);
        }



        public async Task SendOfferNoticeAsync(OfferNoticeDTO model)
        {
            var message = new MessageModel
            {
                Subject = "Offer Notice",
                To = model.CustomerEmail,
                Body = GenerateNotice(model)
            };
            await SendEmailAsync(message);
        }



        public string GenerateNotice(OfferNoticeDTO model)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<html>");
            sb.Append("<head>");
            sb.Append("<style>");
            sb.Append("table { border-collapse: collapse; width: 100%; }");
            sb.Append("th, td { text-align: center; padding: 8px; }");
            sb.Append("th { background-color: #4CAF50; color: white; }");
            sb.Append("td { border: 1px solid black; }");
            sb.Append("</style>");
            sb.Append("</head>");

            sb.Append("<body>");
            sb.AppendFormat("<h1>{0}</h1>", "Hajur Ko Car Rental");
            sb.AppendFormat("<p>{0}</p>", model.Message);

            sb.Append("<table>");
            sb.Append("<tr><th>Car Name</th><th>Offer Price</th><th>Actual Price</th></tr>");
            sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</tr>", model.carName, model.OfferPrice, model.ActualAmount);
            sb.Append("</table>");

            sb.Append("</body>");
            sb.Append("</html>");
            return sb.ToString();

        }

        public string GenerateInvoice(GenerateInvoiceDTO model)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<html>");
            sb.Append("<head>");
            sb.Append("<style>");
            sb.Append("table { border-collapse: collapse; width: 100%; }");
            sb.Append("th, td { text-align: center; padding: 8px; }");
            sb.Append("th { background-color: #4CAF50; color: white; }");
            sb.Append("td { border: 1px solid black; }");
            sb.Append("</style>");
            sb.Append("</head>");

            sb.Append("<body>");
            sb.AppendFormat("<h1>{0}</h1>", "Hajur Ko Car Rental");
            sb.AppendFormat("<p>{0}</p>", model.Message);
            sb.AppendFormat("<p>Invoice Date: {0}</p>", DateTime.Now.ToString("yyyy-MM-dd"));
            sb.AppendFormat("<p>Customer Name: {0}</p>", model.CustomerName);
            sb.AppendFormat("<p>Customer Email: {0}</p>", model.CustomerEmail);

            sb.Append("<table>");
            sb.Append("<tr><th>Car Name</th><th>Rate Per Day</th><th>Rent Start Date</th><th>Rent End Date</th></tr>");
            sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", model.CarName, model.RatePerDay, model.RentStartDate.ToString("yyyy-MM-dd"), model.RentEndDate.ToString("yyyy-MM-dd"));
            sb.Append("</table>");

            sb.AppendFormat("<p>Rental Amount: Rs.{0}</p>", model.RentalAmount);
            sb.AppendFormat("<p>VAT: Rs.{0}</p>", model.VATAmount);
            sb.AppendFormat("<p>Total Amount: Rs.{0}</p>", model.RentalAmount + model.VATAmount);

            sb.Append("</body>");
            sb.Append("</html>");

            return sb.ToString();
        }

    }
}
