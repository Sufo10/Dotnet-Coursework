using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarReportController : ControllerBase
    {
        private readonly ICarReport _carReport;

            public CarReportController(ICarReport carReport)
        {
            _carReport = carReport;
        }

        [Authorize(Roles ="Admin,Staff")]
        [HttpGet]
        [Route("/api/reports/car")]
        public async Task<ResponseDataDTO<List<CarReportDTO>>> GetActiveCars()
        {
            var data = await _carReport.GetActiveCars();
            return data;
        }
    }
}
