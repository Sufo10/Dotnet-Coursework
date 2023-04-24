using System;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace Coursework.Infrastructure.Services
{
	public class CarTestService: ICarTestDetails
    {
        private readonly IApplicationDBContext _dbContext;
        public CarTestService(IApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
        }
        
        public async Task<ResponseDataDTO<List<CarTestDTO>>> GetCarTest()
        {
            try
            {
                var data = _dbContext.Car.Select(e => new CarTestDTO()
                {
                    Name = e.Name,
                }).ToList();

                //Console.WriteLine(data1);

                //var data1 = _dbContext.CustomerBooking
                //.Join(_dbContext.Car,
                //      car => car.Id,
                //      booking => booking.CarId,
                //      (car, booking) => new
                //      {
                //          Car = car,
                //          Booking = booking
                //      })
                //.Where(x => x.Booking.CustomerId == "04059dcc-d5cb-4378-97f6-edce0fa1930d" && x.Booking.IsApproved)
                //.OrderByDescending(x => x.Booking.RentStartdate)
                //.Select(x => x.Car)
                //.ToList();

                return new ResponseDataDTO<List<CarTestDTO>> { Status = "Success", Message = "Data Fetched Successully", Data = data };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new ResponseDataDTO<List<CarTestDTO>> { Status = "Failed", Message = "Data Fetch Failed", Data = { } };
            }
        }
    }
}

