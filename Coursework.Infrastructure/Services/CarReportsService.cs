using System;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Coursework.Infrastructure.Services
{
	public class CarReportsService:ICarReport
	{
        private readonly IApplicationDBContext _dbContext;

        public CarReportsService(IApplicationDBContext dBContext)
		{
            _dbContext = dBContext;

        }
        public async Task<ResponseDataDTO<List<CarReportDTO>>> GetActiveCars()
        {
            try
            {
                var baseUrl = "https://localhost:7190/images/";

                var data = from car in _dbContext.Car
                           select new CarReportDTO
                           {
                               Id = car.Id,
                               Name = car.Name,
                               IsAvailable = car.IsAvailable,
                               Description = car.Description,
                               Image =baseUrl+ car.Image, // setting image with base url
                               RatePerDay = car.RatePerDay,
                               ActualPrice = car.ActualPrice,
                               NumberOfRentals = _dbContext.CustomerBooking.Count(booking => booking.CarId == car.Id.ToString() && booking.OnRent == true)
                           };

                return new ResponseDataDTO<List<CarReportDTO>> { Data = data.ToList(), Status = "Success" };
            }
            catch (Exception err)
            {
                return new ResponseDataDTO<List<CarReportDTO>> { Data = null, Message = err.Message.ToString(), Status = "Error" };
            }
        }
    }
}

