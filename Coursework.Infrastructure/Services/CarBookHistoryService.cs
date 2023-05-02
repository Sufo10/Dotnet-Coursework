using System;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
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
using System.Security.Cryptography;

namespace Coursework.Infrastructure.Services
{
    public class CarBookHistoryService : ICarBookingHistory
    {
        private readonly IApplicationDBContext _dbContext;

        private readonly IEmailService _emailService;
        private readonly UserManager<AppUser> _userManager;

        public CarBookHistoryService(IApplicationDBContext dBContext, UserManager<AppUser> userManager, IEmailService emailService)
        {
            _dbContext = dBContext;
            _emailService = emailService;
            _userManager = userManager;
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

        public async Task<ResponseDataDTO<IEnumerable<SalesRecordResponseDTO>>> GetSalesRecord()
        {
            try
            {
                var salesRecord = from booking in _dbContext.CustomerBooking
                                  join car in _dbContext.Car on booking.CarId equals car.Id.ToString()
                                  join customer in _dbContext.Customer on booking.customerId equals customer.UserId into custGroup
                                  from customer in custGroup.DefaultIfEmpty()
                                  join employee in _dbContext.Employee on booking.ApprovedBy equals employee.UserId into empGroup
                                  from employee in empGroup.DefaultIfEmpty()
                                  where booking.isDeleted != true && (booking.IsComplete == false && booking.OnRent == true) || (booking.IsComplete == true && booking.OnRent == false)
                                  select new SalesRecordResponseDTO
                                  {
                                      Id = booking.Id,
                                      CarName = car.Name,
                                      ApprovedBy = employee != null ? employee.Name : "Unknown",
                                      CustomerName = (customer != null ? customer.Name : null) ?? (employee != null ? employee.Name : null),
                                      Amount = booking.TotalAmount,
                                      CustomerId = customer.Id.ToString() ?? employee.Id.ToString(),
                                      StartDate = booking.RentStartdate,
                                      EndDate = booking.RentEnddate,
                                  };

                var result = await salesRecord.ToListAsync();
                    return new ResponseDataDTO<IEnumerable<SalesRecordResponseDTO>> { Status = "Success", Message = "Data Fetched Successully", Data = result };
                }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new ResponseDataDTO<IEnumerable<SalesRecordResponseDTO>> { Status = "Error", Message = "Data Fetch Failed", Data = { } };
            }
        }

        public async Task<ResponseDTO> AddAdditionalCharges(AdditionalChargeRequestDTO model)
        {
            try
            {
                string uId = _dbContext.CustomerBooking.Where(u => u.Id.ToString() == model.BookingId).Select(u => u.customerId).FirstOrDefault();

                var newAddChg = new AdditionalCharges  //additional charge object
                {
                    BookingId = model.BookingId,
                    CarId = model.CarId,
                    UserId = uId,
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

                string carName= _dbContext.Car.Where(u => u.Id.ToString() == bookingToUpdate.CarId).Select(u => u.Name).FirstOrDefault();

                //string usrId = _dbContext.AdditionalCharges.Where(u => u.Id.ToString() == chargeID).Select(u => u.UserId).FirstOrDefault();

                string desc = _dbContext.AdditionalCharges.Where(u => u.Id.ToString() == chargeID).Select(u => u.Description).FirstOrDefault();

                string ct = _dbContext.AdditionalCharges.Where(u => u.Id.ToString() == chargeID).Select(u => u.CreatedAt).FirstOrDefault().ToString();

                var user = await _userManager.FindByIdAsync(userID);
                if (user != null)
                {
                    userEmail = user.Email;
                }

                await _emailService.SendEmailAdditionalChargesAsync(amount.ToString(), desc, bId, carName, user.UserName, ct, userEmail);
                
                return new ResponseDTO { Status = "Success", Message = "Additional Charges Invoice Sent" };
            }
            catch (Exception err)
            {
                return new ResponseDTO { Status = "Error", Message = err.ToString() };

            }
        }

        public async Task<ResponseDataDTO<IEnumerable<AdditionalChargetDTO>>> GetAdditionalCharges(string userEmail)
        {
            try
            {
                var user = await  _userManager.FindByEmailAsync(userEmail);
                var id = user.Id.ToString();
                var data = await _dbContext.AdditionalCharges
                .Where(ac => ac.UserId == id)
                .Join(_dbContext.Car,
                    ac => ac.CarId,
                    c => c.Id.ToString(),
                    (ac, c) => new AdditionalChargetDTO
                    {
                        Id = ac.Id.ToString(),
                        CarId = c.Name, // Get car name from the Cars table
                        BookingId = ac.BookingId,
                        Description = ac.Description,
                        Amount = (float)ac.Amount,
                        IsPaid = ac.IsPaid
                    })
                .ToListAsync();

                return new ResponseDataDTO<IEnumerable<AdditionalChargetDTO>> { Status = "Success", Message = "Data Fetched Successully", Data = data };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new ResponseDataDTO<IEnumerable<AdditionalChargetDTO>> { Status = "Failed", Message = "Data Fetch Failed", Data = { } };
            }
        }

        public async Task<ResponseDataDTO<IEnumerable<AdditionalChargetDTO>>> GetAdditionalCharges2()
        {
            try
            {
                var data = await _dbContext.AdditionalCharges
                .Join(_dbContext.Car, ac => ac.CarId, c => c.Id.ToString(), (ac, c) => new AdditionalChargetDTO
                {
                    Id = ac.Id.ToString(),
                    CarId = c.Name,
                    BookingId = ac.BookingId,
                    Description = ac.Description,
                    Amount = (float)ac.Amount,
                    IsPaid = ac.IsPaid
                })
                .ToListAsync();

                return new ResponseDataDTO<IEnumerable<AdditionalChargetDTO>> { Status = "Success", Message = "Data Fetched Successully", Data = data };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new ResponseDataDTO<IEnumerable<AdditionalChargetDTO>> { Status = "Failed", Message = "Data Fetch Failed", Data = { } };
            }
        }
    }
}

