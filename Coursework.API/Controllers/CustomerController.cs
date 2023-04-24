using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Coursework.API.Controllers
{
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerDetails _customerDetails;
        public CustomerController(ICustomerDetails customerDetails)
        {
            _customerDetails = customerDetails;
        }

        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        [Route("/api/upload-document")]
        public async Task<ResponseDTO> UploadDocument([FromForm] CustomerFileUploadDTO model)
        {
            var userID = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var data = await _customerDetails.UploadDocument(model, userID);
            return data;
        }
    }
}
