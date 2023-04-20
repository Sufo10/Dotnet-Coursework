using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        public async Task<ResponseDTO> AddCustomer([FromForm]CustomerRegisterRequestDTO model)
        {
            var data = await _authenticate.Register(model);
            return data;
        }
    }
}
