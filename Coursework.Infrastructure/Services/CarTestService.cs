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

                // booking car booking history for a given user.
                //var data = _dbContext.CustomerBooking
                var data = _dbContext.BookingHistory
                    .Join(_dbContext.Car, bh => bh.CarId, c => c.Id, (bh, c) => new { Booking = bh, Car = c })
                    .Where(joinedData => joinedData.Booking.customerId == "04059dcc-d5cb-4378-97f6-edce0fa1930d")
                    .OrderByDescending(joinedData => joinedData.Booking.RentStartdate)
                    .Select(joinedData => new {
                        joinedData.Booking.Id,
                        joinedData.Car.Name,
                        joinedData.Car.IsAvailable,
                        joinedData.Car.RatePerDay,
                        joinedData.Car.Image,
                        joinedData.Car.Description,
                        joinedData.Booking.ApprovedBy,
                        joinedData.Booking.RentStartdate,
                        joinedData.Booking.RentEnddate,
                        joinedData.Booking.IsApproved,
                        joinedData.Booking.CreatedAt,
                        joinedData.Booking.LastModified,
                        joinedData.Booking.DeletedTime,
                        joinedData.Booking.CreatedBy,
                        joinedData.Booking.LastModifiedBy,
                        joinedData.Booking.DeletedBy,
                        joinedData.Booking.isDeleted
                    })
                    .ToList();
                // end block

                return new ResponseDataDTO<List<CarTestDTO>> { Status = "Success", Message = "Data Fetched Successully", Data = data };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}

