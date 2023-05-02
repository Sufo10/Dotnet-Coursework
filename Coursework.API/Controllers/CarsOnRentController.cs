using System.Data;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.API.Controllers
{

    [ApiController]
    public class CarsOnRentController : ControllerBase
    {
        private readonly ICarsOnRent _rcars;

        public CarsOnRentController(ICarsOnRent rcars)
        {
            _rcars = rcars;
        }

        [Authorize(Roles = "Admin,Staff")]
        [Route("/api/rentCar/{BookingId}")]
        public async Task<ResponseDTO> BookCarRequesttt(string BookingId)
        {
            var data = await _rcars.RentCars(BookingId);
            return data;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPatch]
        [Route("/api/ReturnCar/{BookingId}")]
        public async Task<ResponseDTO> RemoveRent(string BookingId)
        {
            var data = await _rcars.RemoveRent(BookingId);
            return data;
        }
    }
}
