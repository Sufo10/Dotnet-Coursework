using System;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Coursework.Infrastructure.Services;
using System.Collections;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Coursework.Infrastructure.Services
{
    public class CarBookHistoryService : ICarBookingHistory
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IApplicationDBContext _dbContext;
        private readonly IEmailService _emailService;
        public CarBookHistoryService(UserManager<AppUser> userManager, IApplicationDBContext dBContext, IEmailService emailService)
        {
            _userManager = userManager;
            _dbContext = dBContext;
            _emailService = emailService;
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

        public async Task<ResponseDTO> AddAdditionalCharges(AdditionalChargeRequestDTO model)
        {
            try
            {
                var newAddChg = new AdditionalCharges
                {
                    BookingId = model.BookingId,
                    CarId = model.CarId,
                    UserId = "98310c32-dd5f-4731-ba46-2fc13b4d742c",
                    Description = model.Description
                };

                _dbContext.AdditionalCharges.AddAsync(newAddChg);
                await _dbContext.SaveChangesAsync(default(CancellationToken));
                
                return new ResponseDTO { Status = "Success", Message = "Additional Charges Successfully Added" };

           }
            catch (Exception err)
            {
               return new ResponseDTO { Status = "Error", Message = err.ToString() };

            }
        }

        public async Task<ResponseDTO> AddAdditionalChargesUpdate(string chargeID, float amount)
        {
            try
            {
                var bookingToUpdate = _dbContext.AdditionalCharges.FirstOrDefault(b => b.Id.ToString() == chargeID);

                if (bookingToUpdate != null)
                {
                    bookingToUpdate.Amount = amount;
                    await _dbContext.SaveChangesAsync(default(CancellationToken));
                }
                //get booking id form add chg table by id
                string bId = _dbContext.AdditionalCharges.Where(u => u.Id.ToString() == chargeID).Select(u => u.BookingId).FirstOrDefault();

                //get customer id/ user id by booking_id form customer book
                string cId = _dbContext.CustomerBooking.Where(u => u.Id.ToString() == bId).Select(u => u.customerId).FirstOrDefault();

                string userEmail = "";
                string userID = cId;

                string carId = _dbContext.AdditionalCharges.Where(u => u.Id.ToString() == chargeID).Select(u => u.CarId).FirstOrDefault();

                string usrId = _dbContext.AdditionalCharges.Where(u => u.Id.ToString() == chargeID).Select(u => u.UserId).FirstOrDefault();

                string desc = _dbContext.AdditionalCharges.Where(u => u.Id.ToString() == chargeID).Select(u => u.Description).FirstOrDefault();

                string ct = _dbContext.AdditionalCharges.Where(u => u.Id.ToString() == chargeID).Select(u => u.CreatedAt).FirstOrDefault().ToString();

                var user = await _userManager.FindByIdAsync(userID);
                if (user != null)
                {
                    userEmail = user.Email;
                }

                await _emailService.SendEmailAdditionalChargesAsync(amount.ToString(), desc, bId, carId, usrId, ct, userEmail);
                
                return new ResponseDTO { Status = "Success", Message = "Additional Charges Invoice Sent" };
            }
            catch (Exception err)
            {
                return new ResponseDTO { Status = "Error", Message = err.ToString() };

            }
        }

    }
}

