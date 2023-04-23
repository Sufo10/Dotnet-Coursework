using System;
using System.Collections.Generic;
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
        public AuthenticationController(ICustomerDetails customerDetails, IAuthenticate authenticate, RoleManager<IdentityRole> roleManager)
        {
            _customerDetails = customerDetails;
            _authenticate = authenticate;
            _roleManager = roleManager;
        }
        [HttpGet]
        [Route("api/customer/all-customer")]
        public async Task<List<Customer>> GetAllCustomerDetails()
        {
            var data = await _customerDetails.GetAllCustomerService();
            return data;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        //[RequestSizeLimit(100_000_000)] // 100 MB limit
        [Route("/api/register")]
        public async Task<ResponseDTO> AddCustomer([FromForm] CustomerRegisterRequestDTO model)
        {
            var data = await _authenticate.Register(model);
            return data;
        }

        [HttpPost]
        [Route("/api/login")]
        public async Task<LoginResponseDTO> Login([FromBody] LoginRequestDTO login)
        {
            var data = await _authenticate.TokenLoginAsync(login);
            return data;
        }


        //incomplete
        [HttpPost]
        [Route("/api/forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            await _authenticate.ForgotPasswordAsync(email);
            return Ok();
        }

        [HttpGet]
        [Route("/api/confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            await _authenticate.ConfirmEmailAsync(userId, token);
            return Redirect("https://www.google.com");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("/api/admin/register-employee")]
        public async Task<ResponseDTO> RegisterEmployee(EmployeeRegistrationRequestDTO model)
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var data = await _authenticate.EmployeeRegister(model, userID);
            return data;
        }
    }
}
