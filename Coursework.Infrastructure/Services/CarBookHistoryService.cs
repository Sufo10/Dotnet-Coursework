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
                  .Where(b => b.customerId == id && b.isDeleted == null) // boooking history wont be displayed if they've deleted it 
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
                       ApprovedBy = b.ApprovedBy, // it is an staff id at this point
                   });

                var leftJoin = from b in innerJoin
                               join c in _dbContext.Employee on b.ApprovedBy equals c.Id.ToString() into cGroup
                               from c in cGroup.DefaultIfEmpty()
                               select new BookingHistoryResponseDTO()
                               {
                                   Id = b.Id,
                                   Name = b.Name,
                                   Image = b.Image,
                                   Description = b.Description,
                                   RentStartdate = b.RentStartdate,
                                   RentEnddate = b.RentEnddate,
                                   ApprovedBy = c != null ? c.Name : string.Empty
                               };


                return new ResponseDataDTO<IEnumerable<BookingHistoryResponseDTO>> { Status = "Success", Message = "Data Fetched Successully", Data = leftJoin };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new ResponseDataDTO<IEnumerable<BookingHistoryResponseDTO>> { Status = "Failed", Message = "Data Fetch Failed", Data = { } };
            }
        }

        public async Task<ResponseDataDTO<IEnumerable<SalesRecordResponseDTO>>> GetSalesRecord(DateTime startDate, DateTime endDate)
        {
            try
            { // sales record for admin (if request is approved and is not deleted, it counts as sales)
                var innerJoin = _dbContext.CustomerBooking
                  .Where(b => b.IsApproved == true && b.isDeleted == null && b.RentStartdate >= startDate && b.RentStartdate <= endDate) // data filters 
                  .Join(// outer sequence 
                   _dbContext.Car,  // inner sequence 
                   b => b.CarId,    // outerKeySelector
                   c => c.Id.ToString(),  // innerKeySelector
                   (b, c) => new SalesRecordResponseDTO()  // result selector
                   {
                       Id = b.Id,
                       Name = c.Name,
                       ApprovedBy = b.ApprovedBy, // it is an staff id at this point
                   });

                var leftJoin = from b in innerJoin
                               join c in _dbContext.Employee on b.ApprovedBy equals c.Id.ToString() into cGroup
                               from c in cGroup.DefaultIfEmpty()
                               select new SalesRecordResponseDTO()
                               {
                                   Id = b.Id, // sales id (id of CustomerBooking)
                                   Name = b.Name, // name of the car
                                   ApprovedBy = c != null ? c.Name : string.Empty, // staff name that approved the request
                               };


                return new ResponseDataDTO<IEnumerable<SalesRecordResponseDTO>> { Status = "Success", Message = "Data Fetched Successully", Data = leftJoin };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new ResponseDataDTO<IEnumerable<SalesRecordResponseDTO>> { Status = "Failed", Message = "Data Fetch Failed", Data = { } };
            }
        }
    }
}

