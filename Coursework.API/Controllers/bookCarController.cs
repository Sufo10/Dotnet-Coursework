﻿using Coursework.Application.Common.Interface;
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

        [HttpPost]
        [Authorize]
        [Route("/api/bookcars")]
        public async Task<ResponseDTO> BookRequest(BookCarRequestDTO model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var response = await _book.BookCarRequest(model, userEmail);
            return response;
        }


        [HttpGet]
        [Route("/api/car-request")]

        public async Task<ResponseDataDTO<List<GetCarBookingRequestDTO>>> GetBookingList()
        {
            var data = await _book.GetBookCarRequests();
            return data;
        }

        [HttpDelete]
        [Route("/api/car-request/reject")]
        public async Task<ResponseDTO> RejectCarBooking(RejectBookingRequestDTO body)
        {
            var data = await _book.RejectBookingRequest(body.BookingId);
            return data;
        }


        [HttpPost]
        [Authorize]
        [Route("/api/verify_request")]

        public async Task<ResponseDTO> VerifyBooking(BookingApproveRequestDTO model)
        {

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var verify = await _book.ApproveBookingRequest(model, userEmail);
            return verify;
        }


    } 
}
