using Coursework.Application.DTO;
using Coursework.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.Common.Interface
{
    public interface IEmailService
    {
        public Task SendEmailAsync(MessageModel messageDTO);
        public Task SendForgotPasswordEmailAsync(string userName, string toEmail, string passwordResetToken);
        public Task SendEmailConfirmationAsync(string name, string userId, string toEmail, string token);
        public  Task SendPaymentInvoiceAsync(GenerateInvoiceDTO model);

    }
}
