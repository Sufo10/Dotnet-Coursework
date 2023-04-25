using System;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace Coursework.Infrastructure.Services
{
    public class CarDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public double RatePerDay { get; set; }
    }
    public class CarBookDTO
    {
        public Guid Id { get; set; }
        public string customerId { get; set; }
        public string CarId { get; set; }
        public DateTime RentStartdate { get; set; }
        public DateTime RentEnddate { get; set; }
    }
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


                var baseUrl = "https://localhost:7190/images/";
                IList<CarDTO> car = _dbContext.Car.Select(e => new CarDTO()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Image = baseUrl + e.Image,
                    Description = e.Description,
                    RatePerDay = e.RatePerDay,
                }).ToList();
                IList<CarBookDTO> book = _dbContext.CustomerBooking.Select(e => new CarBookDTO()
                {
                    Id = e.Id,
                    CarId = e.CarId,
                    customerId = e.customerId,
                    RentStartdate = e.RentStartdate,
                    RentEnddate = e.RentEnddate,
                }).ToList();

                var innerJoin = book.Join(// outer sequence 
                      car,  // inner sequence 
                      b => b.CarId,    // outerKeySelector
                      c => c.Id.ToString(),  // innerKeySelector
                      (b, c) => new BookingHistoryResponseDTO()  // result selector
                      {
                          Id = b.Id,
                          Name = c.Name,
                          Image = c.Image,
                          Description = c.Description,
                          RentStartdate = b.RentStartdate,
                          RentEnddate = b.RentEnddate,
                      });
                Console.WriteLine(innerJoin);

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

