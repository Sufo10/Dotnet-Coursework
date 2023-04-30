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

        [HttpGet("/api/sales-record/{startDate}/{endDate}")]
        public async Task<ResponseDataDTO<IEnumerable<SalesRecordResponseDTO>>> GetCarHistoryy(DateTime startDate, DateTime endDate)
        {
            var data = await _carDetails.GetSalesRecord(startDate, endDate);
            return data;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("/api/additional-charge")]
        public async Task<ResponseDTO> AddAdditionalCharges([FromForm] AdditionalChargeRequestDTO model)
        {
            var data = await _carDetails.AddAdditionalCharges(model);
            return data;
        }

        [HttpPatch]
        [Consumes("multipart/form-data")]
        [Route("/api/additional-charge")]
        public async Task<ResponseDTO> AddAdditionalChargesUpdate(string chargeID, float amount)
        {
            var data = await _carDetails.AddAdditionalChargesUpdate(chargeID, amount);

            return data;
        }
    }
}
