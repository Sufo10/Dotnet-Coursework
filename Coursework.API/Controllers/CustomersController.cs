using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.API.Controllers
{
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerDetails _customerDetails;
        public CustomersController(ICustomerDetails customerDetails)
        {
            _customerDetails = customerDetails;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("/api/upload-document")]
        public async Task<ResponseDTO> UploadDocument([FromForm]CustomerFileUploadDTO model)
        {
            var userID = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var data = await _customerDetails.UploadDocument(model,userID);
            return data;
        }
    }
}
