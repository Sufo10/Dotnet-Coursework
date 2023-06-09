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
    public class CarContollerController : ControllerBase
    {
        private readonly ICarDetails _carDetails;
        private readonly IWebHostEnvironment _environment;

        public CarContollerController(ICarDetails carDetails, IWebHostEnvironment environment)
        {
            _carDetails = carDetails;
            _environment = environment;
        }

        [HttpGet]
        [Route("/api/cars")]
        public async Task<ResponseDataDTO<List<CarUserDTO>>> GetAllCars()
        {
            var data = await _carDetails.GetActiveCars();
            return data;
        }

        [Authorize(Roles ="Admin,Staff")]
        [HttpGet]
        [Route("/api/trash-cars")]
        public async Task<ResponseDataDTO<List<CarUserDTO>>> GetTrashCars()
        {
            var data = await _carDetails.GetTrashCars();
            return data;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("/api/cars")]
        public async Task<ResponseDTO> AddCar([FromForm] EditCarRequestDTO model)
        {
            var data = await _carDetails.AddCars(model);
            return data;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPatch("/api/editcar/{id}")]
        [Consumes("multipart/form-data")]
        //[Route("/api/editcar")]
        public async Task<ResponseDTO> EditCar(Guid id, [FromForm] CarEditDTO model)
        {
            var data = await _carDetails.EditCar(id, model);
            return data;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPatch("/api/RatePerDay/{id}")]
        [Consumes("multipart/form-data")]
        //[Route("/api/editcar")]
        public async Task<ResponseDTO> EditRatePerDay(Guid id, [FromForm] RatePerDayDTO model)
        {
            var data = await _carDetails.EditRatePerDay(id, model);
            return data;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpDelete("/api/RemoveCar/{CarId}")]
        public async Task<ResponseDTO> RemoveCarss(string CarId)
        {
            var data = await _carDetails.RemoveCars(CarId);
            return data;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpDelete("/api/RestoreCar/{CarId}")]
        public async Task<ResponseDTO> RestoreCar(string CarId)
        {
            var data = await _carDetails.RestoreCar(CarId);
            return data;
        }


    }
}
