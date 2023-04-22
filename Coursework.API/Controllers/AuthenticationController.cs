using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coursework.API.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ICustomerDetails _customerDetails;
        private readonly IAuthenticate _authenticate;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthenticationController(ICustomerDetails customerDetails, IAuthenticate authenticate, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _customerDetails = customerDetails;
            _authenticate = authenticate;
            _roleManager = roleManager;
            _userManager = userManager;

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
        public async Task<ResponseDTO> AddCustomer([FromForm]CustomerRegisterRequestDTO model)
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

        [Authorize]
        [HttpPost]
        [Route("/api/change-password")]
        public async Task<ResponseDTO> ChangePassword([FromBody] UserChangePasswordDTO model)
        {
            var userID = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var data = await _authenticate.ChangePassword(model, userID);
            return data;
        }
    }
}
