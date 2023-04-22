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
    public class CarTestController : ControllerBase
    {
        private readonly ICarTestDetails _carDetails;
        private readonly IWebHostEnvironment _environment;

        public CarTestController(ICarTestDetails carDetails, IWebHostEnvironment environment)
        {
            _carDetails = carDetails;
            _environment = environment;
        }

        [HttpGet]
        [Route("/api/tcars")]
        public async Task<ResponseDataDTO<List<CarTestDTO>>> GetTestCars()
        {
            var data = await _carDetails.GetCarTest();
            return data;
        }

        
    }
}
