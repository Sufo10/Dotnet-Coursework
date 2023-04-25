﻿using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
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

        [HttpPost]
        [Route("/api/bookcars")]
        public async Task<ResponseDTO> BookRequest(BookCarRequestDTO model)
        {
            var userID = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _book.BookCarRequest(model, userID);
            return response;
        }


        [HttpGet]
        [Route("/api/car-request")]

        public async Task<ResponseDataDTO<List<GetCarBookingRequestDTO>>> GetBookingList()
        {
            var data = await _book.GetBookCarRequests();
            return data;
        }


        //[HttpPost]
        //[Route("/api/verify_request")]

        //public async Task<ResponseDTO> VerifyBooking(BookingApproveRequestDTO mode)
        //{
        //    var userID = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        //}


    } 
}
