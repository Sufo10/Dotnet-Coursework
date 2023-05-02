using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.API.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ICustomerDetails _customerDetails;
        private readonly IAuthenticate _authenticate;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly string _baseUrl;

        public AuthenticationController(IConfiguration configuration, ICustomerDetails customerDetails, IAuthenticate authenticate, RoleManager<IdentityRole> roleManager)
        {
            _customerDetails = customerDetails;
            _authenticate = authenticate;
            _roleManager = roleManager;
            _baseUrl = configuration.GetSection("BaseUrl:Frontend").Value!;

        }
       
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("/api/register")]
        public async Task<ResponseDTO> AddCustomer([FromForm] CustomerRegisterRequestDTO model)
        {
            var data = await _authenticate.Register(model);
            return data;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/api/login")]
        public async Task<LoginResponseDTO> Login([FromBody] LoginRequestDTO login)
        {
            var data = await _authenticate.TokenLoginAsync(login);
            return data;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/api/forgot-password")]
        public async Task<ResponseDTO> ForgotPassword(ForgotPasswordRequestDTO body)
        {
            var data = await _authenticate.ForgotPasswordEmailAsync(body.Email);
            return data;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/api/reset-password")]
        public async Task<ResponseDTO> ResetPassword(ResetPasswordRequestDTO body)
        {
            Console.WriteLine(body);
            var data = await _authenticate.ResetPasswordAsync(body);
            return data;
        }

        [HttpGet]
        [Route("/api/confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
           var data = await _authenticate.ConfirmEmailAsync(userId, token);
            if(data.Status == "Already Confirmed")
            {
                return Redirect($"{_baseUrl}email-confirmed?confirmed=already");
            }
            else
            {
                return Redirect($"{_baseUrl}email-confirmed?confirmed=true");
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [Route("/api/admin/register-employee")]
        public async Task<ResponseDTO> RegisterEmployee(EmployeeRegistrationRequestDTO model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data = await _authenticate.EmployeeRegister(model, userEmail);
            return data;
        }

        [Authorize]
        [HttpPost]
        [Route("/api/change-password")]
        public async Task<ResponseDTO> ChangePassword([FromBody] UserChangePasswordDTO model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var data = await _authenticate.ChangePassword(model, userEmail);
            return data;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        [Route("/api/employees")]
        public async Task<ResponseDataDTO<List<EmployeeResponseDTO>>> GetAllEmployee()
        {
            var data = await _authenticate.GetAllEmployee();
            return data;
        }
    }
}
