using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
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
        [HttpGet("/api/car-booking-history/{id}")]
        public async Task<ResponseDataDTO<IEnumerable<BookingHistoryResponseDTO>>> GetCarHistoryy(string id)
        {
            var data = await _carDetails.GetCarHistory(id);
            return data;
        }

        [HttpGet("/api/sales-record")]
        public async Task<ResponseDataDTO<IEnumerable<SalesRecordResponseDTO>>> GetCarHistory()
        {
            var data = await _carDetails.GetSalesRecord();
            return data;
        }

        [HttpPost]
        [Route("/api/additional-charge")]
        public async Task<ResponseDTO> AddAdditionalCharges(AdditionalChargeRequestDTO model)
        {
            var data = await _carDetails.AddAdditionalCharges(model);
            return data;
        }

        [HttpPatch]
        [Route("/api/additional-charge-send-invoice")]
        public async Task<ResponseDTO> AddAdditionalChargesUpdate(AdditionalChargeUpRequestDTO model)
        {
            var data = await _carDetails.AddAdditionalChargesUpdate(model.ChargeId, model.Amount);

            return data;
        }

        [HttpGet("/api/additional-charges/{id}")]
        public async Task<ResponseDataDTO<IEnumerable<AdditionalChargetDTO>>> GetAddiChgs(string id)
        {
            var data = await _carDetails.GetAdditionalCharges(id);
            return data;
        }

        [HttpGet("/api/all-additional-charges")]
        public async Task<ResponseDataDTO<IEnumerable<AdditionalChargetDTO>>> GetAddiChgs2()
        {
            var data = await _carDetails.GetAdditionalCharges2();
            return data;
        }
    }
}
