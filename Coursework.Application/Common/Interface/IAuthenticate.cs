using System;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Http;

namespace Coursework.Application.Common.Interface
{
    public interface IAuthenticate
    {
        Task<ResponseDTO> Register(CustomerRegisterRequestDTO model);
        Task<LoginResponseDTO> TokenLoginAsync(LoginRequestDTO model);
        Task ForgotPasswordAsync(string email);
        Task ConfirmEmailAsync(string userId, string token);
        Task<ResponseDTO> EmployeeRegister(EmployeeRegistrationRequestDTO model, string userID);
        Task<ResponseDTO> ChangePassword(UserChangePasswordDTO model, Guid userID);
    }
}

