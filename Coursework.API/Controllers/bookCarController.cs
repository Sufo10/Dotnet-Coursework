using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Coursework.API.Controllers
{
    [ApiController]
    public class BookCarController : ControllerBase 
    {
        private readonly IBookCar _book;

        public BookCarController(IBookCar book)
        {
            _book = book;
        }

        [Authorize]
        [HttpPost]
        [Route("/api/bookcars")]
        public async Task<ResponseDTO> BookRequest(BookCarRequestDTO model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var response = await _book.BookCarRequest(model, userEmail);
            return response;
        }


        [Authorize]
        [HttpGet]
        [Route("/api/car-request")]

        public async Task<ResponseDataDTO<List<GetCarBookingRequestDTO>>> GetBookingList()
        {
            var data = await _book.GetBookCarRequests();
            return data;
        }

        [Authorize]
        [HttpDelete]
        [Route("/api/car-request/{BookingId}")]
        public async Task<ResponseDTO> RejectCarBooking(String BookingId)
        {
            var data = await _book.RejectBookingRequest(BookingId);
            return data;
        }


        [Authorize]
        [HttpPost]
        [Route("/api/verify-request/{bookingId}")]

        public async Task<ResponseDTO> VerifyBooking(String bookingId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var verify = await _book.ApproveBookingRequest(bookingId, userEmail);
            return verify;
        }


    } 
}
