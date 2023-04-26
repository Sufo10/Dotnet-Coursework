using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Coursework.Infrastructure.Services
{
    public class BookCarService : IBookCar


    {
        private readonly IApplicationDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public BookCarService(IApplicationDBContext dBContext, UserManager<AppUser> userManager)
        {
            _dbContext = dBContext;
            _userManager = userManager;
        }

        //public async Task<ResponseDTO> ApproveBookingRequest(BookingApproveRequestDTO model, Guid userID)
        //{

        //    try
        //    {
        //        var entityToUpdate = await _dbContext.CustomerBooking.SingleOrDefaultAsync(c => c.Id == Guid.Parse(model.BookingId));
        //        string customerId = model.customerId; 
        //        var latestBooking = await _dbContext.CustomerBooking
        //            .Where(b => b.customerId == customerId && (bool)b.IsApproved)
        //            .OrderByDescending(b => b.RentStartdate)
        //            .FirstOrDefaultAsync();

        //        DateTime today = DateTime.Today;
        //        DateTime threeMonthsAgo = today.AddMonths(-3);

        //        bool regular = false;

        //        if (latestBooking.RentStartdate >= threeMonthsAgo)
        //        {
        //            regular = true;
        //        }




        //            if (entityToUpdate != null)
        //        {
        //            var carRate = await _dbContext.CustomerBooking
        //                .Join(_dbContext.Car,
        //                    c => c.CarId.ToString(),
        //                    cr => cr.Id.ToString(),
        //                    (c,cr) => new { rate = cr.RatePerDay }).ToListAsync();


        //            if (regular = true)
        //            {
        //                var rentDays = (latestBooking.RentEnddate - latestBooking.RentStartdate).TotalDays + 1;
        //                var totalAmount = carRate.FirstOrDefault().rate * rentDays;
        //                var totalAfterDiscount = totalAmount * 0.9;

        //            }

        //            else
        //            {
        //                var rentDays = (latestBooking.RentEnddate - latestBooking.RentStartdate).TotalDays + 1;
        //                var totalAmount = carRate.FirstOrDefault().rate * rentDays;
        //                var totalAfterDiscount = totalAmount;
        //            }


        //        }


        //        return new ResponseDTO() { Status = "Success", Message = "Booking request approved" };
        //    }

        //    catch (Exception ex) 
        //    {
        //        return new ResponseDTO() { Status = "unsuccessful", Message = ex.ToString() };
        //    }

        //}


        public async Task<ResponseDTO> ApproveBookingRequest(BookingApproveRequestDTO model, Guid userID)
        {
            try
            {
                var entityToUpdate = await _dbContext.CustomerBooking.SingleOrDefaultAsync(c => c.Id == Guid.Parse(model.BookingId));
                var customerId = model.customerId;
                var latestBooking = await _dbContext.CustomerBooking
                    .Where(b => b.customerId == customerId && (bool)b.IsApproved)
                    .OrderByDescending(b => b.RentStartdate)
                    .FirstOrDefaultAsync();

                var today = DateTime.Today;
                var threeMonthsAgo = today.AddMonths(-3);

                var regular = latestBooking.RentStartdate >= threeMonthsAgo;

                var carRate = await _dbContext.Car
                 .Where(c => c.Id == Guid.Parse(entityToUpdate.CarId))
                 .Select(c => c.RatePerDay)
                 .FirstOrDefaultAsync();


                var rentDays = (latestBooking.RentEnddate - latestBooking.RentStartdate).TotalDays + 1;
                var totalAmount = carRate * rentDays;
                var totalAfterDiscount = regular ? totalAmount * 0.9 : totalAmount;

                entityToUpdate.IsApproved = true;
                entityToUpdate.ApprovedBy = userID.ToString();
                _dbContext.CustomerBooking.Update(entityToUpdate);
                await _dbContext.SaveChangesAsync(default(CancellationToken));
                return new ResponseDTO() { Status = "Success", Message = "Booking request approved" };
            }
            catch (Exception ex)
            {
                return new ResponseDTO() { Status = "unsuccessful", Message = ex.ToString() };
            }
        }


        public async Task<ResponseDTO> BookCarRequest(BookCarRequestDTO model, Guid userID)
        {
            try
            {


                var user = await _userManager.FindByIdAsync(userID.ToString());

                var role = await _userManager.GetRolesAsync(user);
                if (role.FirstOrDefault() == "Customer")
                {

                    var customerDetails = await _dbContext.Customer.SingleOrDefaultAsync(c => c.UserId == userID.ToString());

                    if (customerDetails.IsVerified == false)
                    {
                        return new ResponseDTO { Status = "Error", Message = "Customer not verified" };
                    }
                    else
                    {
                        var bookCar = new CustomerBooking
                        {
                            customerId = userID.ToString(),
                            CarId = model.CarId,
                            RentStartdate = model.RentStartdate,
                            RentEnddate = model.RentEnddate
                        };

                        var RequestInput = await _dbContext.CustomerBooking.AddAsync(bookCar);
                        await _dbContext.SaveChangesAsync(default(CancellationToken));
                        return new ResponseDTO { Status = "Success", Message = "Booking request sent" };
                    }
                }
                else if (role.FirstOrDefault() == "employee")
                {

                    var customerDetails = await _dbContext.Employee.SingleOrDefaultAsync(c => c.UserId == userID.ToString());

                    if (customerDetails.IsVerified == false)
                    {
                        return new ResponseDTO { Status = "Error", Message = "Customer not verified" };
                    }
                    else
                    {
                        var bookCar = new CustomerBooking
                        {
                            customerId = userID.ToString(),
                            CarId = model.CarId,
                            RentStartdate = model.RentStartdate,
                            RentEnddate = model.RentEnddate
                        };

                        var RequestInput = await _dbContext.CustomerBooking.AddAsync(bookCar);
                        await _dbContext.SaveChangesAsync(default(CancellationToken));
                        return new ResponseDTO { Status = "Success", Message = "Booking request sent" };
                    }

                }
                else
                {
                    return new ResponseDTO { Status = "unsuccessful", Message = "something went wrong" };
                }
                //return new ResponseDTO { Status = "Success", Message = role.FirstOrDefault() };

            }
            catch (Exception ex)
            {
                return new ResponseDTO { Status = "Error", Message = ex.ToString() };
            }
        }



        public async Task<ResponseDataDTO<List<GetCarBookingRequestDTO>>> GetBookCarRequests()
        {
            try 
            {
                var bookingRequests = await (
                from booking in _dbContext.CustomerBooking
                where booking.isDeleted == false || booking.isDeleted == null
                let employee = _dbContext.Employee.FirstOrDefault(e => e.UserId == booking.customerId)
                let customer = _dbContext.Customer.FirstOrDefault(c => c.UserId == booking.customerId)
                let customerName = employee != null ? employee.Name : customer.Name
                let customerPhone = employee != null ? employee.Phone : customer.Phone
                select new GetCarBookingRequestDTO
                {
                    BookingId = booking.Id.ToString(),
                    CustomerId = booking.customerId.ToString(),
                    CustomerName = customerName,
                    CustomerPhone = customerPhone,
                    CarId = booking.CarId,
                    RentStartdate = booking.RentStartdate,
                    RentEnddate = booking.RentEnddate,
                }
                ).ToListAsync();

                if (bookingRequests.Count == 0)
                {
                    return new ResponseDataDTO<List<GetCarBookingRequestDTO>> { Status = "Error", Message = "No booking requests found", Data = null };
                }

                return new ResponseDataDTO<List<GetCarBookingRequestDTO>> { Status = "Success", Message = "Data Fetched Successfully", Data = bookingRequests };
            }

            catch (Exception ex)
            {
                return new ResponseDataDTO<List<GetCarBookingRequestDTO>> { Status = "Error", Message = ex.ToString(), Data = null };
            }

        }


        public async Task<ResponseDTO> RejectBookingRequest(string bookingId)
        {

            try 
            {
                var entityToUpdate = await _dbContext.CustomerBooking.FindAsync(bookingId);

                if (entityToUpdate == null)
                {
                    return new ResponseDTO() { Status = "Error", Message = "Booking not found" };
                }

                entityToUpdate.isDeleted = true;
                _dbContext.CustomerBooking.Update(entityToUpdate);
                await _dbContext.SaveChangesAsync(default(CancellationToken));
                return new ResponseDTO() { Status = "Success", Message = "Booking request disapproved." };
            }
            
            catch (Exception ex) {
                return new ResponseDTO() { Status = "unsuccessful", Message = ex.ToString() };
            }
        }

    }
}
