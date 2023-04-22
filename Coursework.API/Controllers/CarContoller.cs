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

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Route("/api/cars")]
        public async Task<ResponseDTO> AddCustomer([FromForm] EditCarRequestDTO model)
        {
            var data = await _carDetails.AddCars(model);
            return data;
        }
    }
}
