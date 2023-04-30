using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Security.Claims;

namespace Coursework.API.Controllers
{
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IKhaltiPaymentService _khaltiPaymentService;
        private readonly string _baseUrl;
        public PaymentController(IConfiguration configuration, IKhaltiPaymentService khaltiPaymentService)
        {
            _khaltiPaymentService = khaltiPaymentService;
            _baseUrl = configuration.GetSection("BaseUrl:Frontend").Value!;
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

        //[HttpPost]
        //[Route("/api/khalti-payment/status")]
        //public async Task<ResponseDataDTO<KhaltiResponseDTO>> KhaltiPaymentStatus(KhaltiPaymentCheckDTO model)
        //{
        //    var data = await _khaltiPaymentService.CheckPaymentSuccess(model);
        //    return data;
        //}

        [HttpGet]
        [Route("/api/khalti-payment/status")]
        public async Task<IActionResult> CheckKhaltiPayment()
        {
            string pidx = HttpContext.Request.Query["pidx"];
            string bookingId = HttpContext.Request.Query["purchase_order_id"]; 
            int amount = int.Parse(HttpContext.Request.Query["amount"]);
            var data = await _khaltiPaymentService.CheckPaymentSuccess(pidx, bookingId, amount);
            if(data.Status == "Success") return Redirect($"{_baseUrl}payment-success");
            else return BadRequest(data);
        }

        [HttpPost]
        //[Authorize (Roles = "Staff")]
        [Route("/api/offline-payment")]
        public async Task<ResponseDataDTO<KhaltiResponseDTO>> OfflinePayment(KhaltiPaymentDTO model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var data = await _khaltiPaymentService.OfflinePayment(model);
            return data;
        }

        [HttpPost]
        //[Authorize (Roles = "Staff")]
        [Route("/api/offline-payment/addtional-charge")]
        public async Task<ResponseDataDTO<KhaltiResponseDTO>> OfflinePaymentForAdditonalCharge(AdditonalChargePaymentDTO model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var data = await _khaltiPaymentService.OfflinePaymentForAdditionalCharges(model);
            return data;
        }
    }
}
