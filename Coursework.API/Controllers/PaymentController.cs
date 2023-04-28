using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Coursework.API.Controllers
{
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IKhaltiPaymentService _khaltiPaymentService;
        public PaymentController(IKhaltiPaymentService khaltiPaymentService)
        {
            _khaltiPaymentService = khaltiPaymentService;
        }

        [HttpPost]
        [Authorize]
        [Route("/api/khalti-payment")]
        public async Task<ResponseDataDTO<KhaltiResponseDTO>> InitializeKhaltiPayment(KhaltiPaymentDTO model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var data = await _khaltiPaymentService.InitializePayment(model, userEmail);
            return data;
        }

        [HttpPost]
        [Route("/api/khalti-payment/status")]
        public async Task<ResponseDataDTO<KhaltiResponseDTO>> CheckKhaltiPaymentSuccess(KhaltiPaymentCheckDTO model)
        {
            var data = await _khaltiPaymentService.CheckPaymentSuccess(model);
            return data;
        }
    }
}
