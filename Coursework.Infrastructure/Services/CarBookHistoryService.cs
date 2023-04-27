using System;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

namespace Coursework.Infrastructure.Services
{
    public class CarBookHistoryService : ICarBookingHistory
    {
        private readonly IApplicationDBContext _dbContext;
        public CarBookHistoryService(IApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
        }
        
        public async Task<ResponseDataDTO<IEnumerable<BookingHistoryResponseDTO>>> GetCarHistory(string id)
        {
            try
            {
                var baseUrl = "https://localhost:7190/images/";
                
                var innerJoin = _dbContext.CustomerBooking
                  .Where(b => b.customerId == id)
                  .Join(// outer sequence 
                   _dbContext.Car,  // inner sequence 
                   b => b.CarId,    // outerKeySelector
                   c => c.Id.ToString(),  // innerKeySelector
                   (b, c) => new BookingHistoryResponseDTO()  // result selector
                   {
                       Id = b.Id,
                       Name = c.Name,
                       Image = baseUrl + c.Image,
                       Description = c.Description,
                       RentStartdate = b.RentStartdate,
                       RentEnddate = b.RentEnddate,
                   });

                return new ResponseDataDTO<IEnumerable<BookingHistoryResponseDTO>> { Status = "Success", Message = "Data Fetched Successully", Data = innerJoin };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new ResponseDataDTO<IEnumerable<BookingHistoryResponseDTO>> { Status = "Failed", Message = "Data Fetch Failed", Data = { } };
            }
        }
    }
}

