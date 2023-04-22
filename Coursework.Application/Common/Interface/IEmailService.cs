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
        Task SendEmailAsync(MessageModel messageDTO);
        Task SendForgotPasswordEmailAsync(string userName, string toEmail, string passwordResetToken);
        Task SendEmailConfirmationAsync(string name, string userId, string toEmail, string token);
    }
}
