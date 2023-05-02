using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.API.Controllers
{
  [ApiController]
    public class CarHistoryController : ControllerBase
    {
        private readonly ICarBookingHistory _carDetails;
        private readonly IWebHostEnvironment _environment;

        public CarHistoryController(ICarBookingHistory carDetails, IWebHostEnvironment environment)
        {
            _carDetails = carDetails;
            _environment = environment;
        }


        // API to
        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("/api/car-booking-history/{id}")]
        public async Task<ResponseDataDTO<IEnumerable<BookingHistoryResponseDTO>>> GetCarHistoryy(string id)
        {
            var data = await _carDetails.GetCarHistory(id);
            return data;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("/api/sales-record")]
        public async Task<ResponseDataDTO<IEnumerable<SalesRecordResponseDTO>>> GetCarHistory()
        {
            var data = await _carDetails.GetSalesRecord();
            return data;
        }

        [Authorize]
        [HttpPost]
        [Route("/api/additional-charge")]
        public async Task<ResponseDTO> AddAdditionalCharges(AdditionalChargeRequestDTO model)
        {
            var data = await _carDetails.AddAdditionalCharges(model);
            return data;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPatch]
        [Route("/api/additional-charge-send-invoice")]
        public async Task<ResponseDTO> AddAdditionalChargesUpdate(AdditionalChargeUpRequestDTO model)
        {
            var data = await _carDetails.AddAdditionalChargesUpdate(model.ChargeId, model.Amount);

            return data;
        }

        [Authorize]
        [HttpGet("/api/additional-charges")]
        public async Task<ResponseDataDTO<IEnumerable<AdditionalChargetDTO>>> GetAddiChgs()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var data = await _carDetails.GetAdditionalCharges(userEmail);
            return data;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("/api/admin/additional-charges")]
        public async Task<ResponseDataDTO<IEnumerable<AdditionalChargetDTO>>> GetAddiChgs2()
        {
            var data = await _carDetails.GetAdditionalCharges2();
            return data;
        }
    }
}
