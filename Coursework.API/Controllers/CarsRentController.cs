using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.API.Controllers
{
    public class CarsRentController : ControllerBase
    {
        private readonly ICarRent _rent;

        public CarsRentController(ICarRent rent)
        {
            _rent = rent;
        }

        [HttpGet]
        [Route ("/api/getPaidCars")]
        public async Task<ResponseDataDTO<List<GetCarBookingRequestDTO>>> GetPaidcars()
        {
            var data = await _rent.GetPaidCars();
            return data;
        }
    }
}
