using System;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Http;

namespace Coursework.Application.Common.Interface
{
    public interface IAuthenticate
    {
        Task<ResponseDTO> Register(CustomerRegisterRequestDTO model);
        Task<LoginResponseDTO> TokenLoginAsync(LoginRequestDTO model);
        Task<ResponseDTO> ForgotPasswordEmailAsync(string email);
        Task ConfirmEmailAsync(string userId, string token);
        Task<ResponseDTO> EmployeeRegister(EmployeeRegistrationRequestDTO model, string userEmail);
        Task<ResponseDTO> ChangePassword(UserChangePasswordDTO model, string email);
        Task<ResponseDTO> ResetPasswordAsync(ResetPasswordRequestDTO body);
        Task<ResponseDataDTO<List<EmployeeResponseDTO>>> GetAllEmployee();
    }
}

