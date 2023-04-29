using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
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

        [HttpPatch]
        [Route("/api/rentCar/{BookingId}")]
        public async Task<ResponseDTO> BookCarRequesttt(string BookingId)
        {
            var data = await _rcars.RentCars(BookingId);
            return data;
        }
    }
}
