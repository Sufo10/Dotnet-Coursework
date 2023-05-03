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
using Coursework.Infrastructure.Services;
using System.Collections;

namespace Coursework.Infrastructure.Services
{
    public class BookCarService : IBookCar


    {
        private readonly IApplicationDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;

        public BookCarService(IApplicationDBContext dBContext, UserManager<AppUser> userManager, IEmailService emailService)
        {
            _dbContext = dBContext;
            _userManager = userManager;
            _emailService = emailService;
        }

       


        public async Task<ResponseDTO> ApproveBookingRequest(String bookingId, string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                string userID = user.Id; //user Id of the staff who is approving the request
                var entityToUpdate = await _dbContext.CustomerBooking.SingleOrDefaultAsync(c => c.Id == Guid.Parse( bookingId));
                var customerId = entityToUpdate.customerId;
                var customer = await _userManager.FindByIdAsync(customerId);
                var customerRole = await _userManager.GetRolesAsync(customer);
                var isStaff = false;

                

                if (customerRole.FirstOrDefault() == "Staff")
                {
                    isStaff = true;
                    

                }
                else if (customerRole.FirstOrDefault() == "Customer")
                {
                    isStaff = false;
                    var customerDetails = await _dbContext.Employee.SingleOrDefaultAsync(c => c.UserId == customerId.ToString());
      
                }
      

                var customerDetails1 = await _dbContext.Customer.SingleOrDefaultAsync(c => c.UserId == customerId.ToString());
                
                var latestBooking = await _dbContext.CustomerBooking  //getting user latest booking
                    .Where(b => b.customerId == customerId && (bool)b.IsApproved)
                    .OrderByDescending(b => b.RentStartdate)
                    .FirstOrDefaultAsync();

                var today = DateTime.Today;
                var threeMonthsAgo = today.AddMonths(-3);

                var regular = false;
                

                if (latestBooking != null && latestBooking.RentStartdate >= threeMonthsAgo)
                {
                    regular = true;  //user is regualr
                }


                var car = await _dbContext.Car
                 .Where(c => c.Id == Guid.Parse(entityToUpdate.CarId))
                 .Select(c => new { c.RatePerDay, c.Name })
                 .FirstOrDefaultAsync();



                var rentDays = (entityToUpdate.RentEnddate - entityToUpdate.RentStartdate).TotalDays + 1 ;
                var totalAmount = car.RatePerDay * rentDays;
                var totalAfterDiscount = regular ? totalAmount * 0.9 : (isStaff ? totalAmount * 0.75 : totalAmount);
                var vatAmount = totalAfterDiscount * 0.13;
                

                entityToUpdate.IsApproved = true;
                entityToUpdate.ApprovedBy = userID;
                _dbContext.CustomerBooking.Update(entityToUpdate);
                await _dbContext.SaveChangesAsync(default(CancellationToken));


                
                
                
                if (isStaff == true) {
                    var customerDetails = await _dbContext.Employee.SingleOrDefaultAsync(c => c.UserId == customerId.ToString());
                    var Invoice = new GenerateInvoiceDTO()
                    {
                        CustomerName = customerDetails.Name,
                        CustomerEmail = customer.Email,
                        CarName = car.Name,
                        RatePerDay = car.RatePerDay,
                        RentStartDate = entityToUpdate.RentStartdate,
                        RentEndDate = entityToUpdate.RentEnddate,
                        RentalAmount = (int)Math.Round(totalAfterDiscount),
                        VATAmount = (int)Math.Round(vatAmount),
                        Message = "Your request has been approved, Please pay the amount shown below:"
                    };

                    _emailService.SendPaymentInvoiceAsync(Invoice);
                    return new ResponseDTO() { Status = "Success", Message = "Booking approved" };

                }
                else if (isStaff == false)
                {
                    var customerDetails = await _dbContext.Customer.SingleOrDefaultAsync(c => c.UserId == customerId.ToString());
                    var Invoice = new GenerateInvoiceDTO()
                    {
                        CustomerName = customerDetails.Name,
                        CustomerEmail = customer.Email,
                        CarName = car.Name,
                        RatePerDay = car.RatePerDay,
                        RentStartDate = entityToUpdate.RentStartdate,
                        RentEndDate = entityToUpdate.RentEnddate,
                        RentalAmount = (int)Math.Round(totalAfterDiscount),
                        VATAmount = (int)Math.Round(vatAmount),
                        Message = "Your request has been approved, Please pay the amount shown below:"
                    };

                    _emailService.SendPaymentInvoiceAsync(Invoice);

                    return new ResponseDTO() { Status = "Success", Message = "Booking approved" };
                }

                else
                {
                    return new ResponseDTO() { Status = "Error", Message = "No roles found" };
                }




            }
            catch (Exception ex)
            {
                return new ResponseDTO() { Status = "Error", Message = ex.Message.ToString() };
            }
        }



        public async Task<ResponseDTO> BookCarRequest(BookCarRequestDTO model,string email)
        {
            try {
                var user =await _userManager.FindByEmailAsync(email);
                var userID = user.Id;
                var userCharges = await _dbContext.AdditionalCharges.FirstOrDefaultAsync(c => c.UserId == userID && c.IsPaid == false);

                if (userCharges != null) 
                {
                    return new ResponseDTO { Status = "Error", Message = "Damage payment not done" };
                }

                var car = await _dbContext.Car.SingleOrDefaultAsync(c => c.Id == Guid.Parse(model.CarId));
                var today = DateTime.Today;
                var threeMonthsAgo = today.AddMonths(-3); //getting user booking

                var latestBooking = await _dbContext.CustomerBooking
                .Where(b => b.customerId == userID && (bool)b.IsApproved)
                .OrderByDescending(b => b.RentStartdate)
                .FirstOrDefaultAsync();

                var regular = false;

                if (latestBooking != null && latestBooking.RentStartdate >= threeMonthsAgo)
                {
                    regular = true;
                }

                var overlappingBookings = _dbContext.CustomerBooking //car is already booked for particular date range
                .Where(cb => cb.CarId == model.CarId
                && cb.RentEnddate >= model.RentStartdate.Date
                && cb.RentStartdate <= model.RentEnddate.Date && cb.isDeleted != true);


                var isAvailable = !overlappingBookings.Any();

                if (isAvailable == false) {
                    return new ResponseDTO { Status = "Error", Message = "Car is not available for the requested date" };
                }
                else {

                    var role = await _userManager.GetRolesAsync(user);

                    if (role.FirstOrDefault() == "Customer") //checking user roel for applying discount
                    {

                        var customerDetails = await _dbContext.Customer.SingleOrDefaultAsync(c => c.UserId == userID.ToString());


                        if (customerDetails.IsVerified == false)
                        {
                            return new ResponseDTO { Status = "Error", Message = "Customer not verified" };
                        }
                        else
                        {
                            
                            var rentDays = (model.RentEnddate - model.RentStartdate).TotalDays + 1; //number of days for rent
                            var totalAmount = car.RatePerDay * rentDays; 
                            var totalAfterDiscount = regular ? totalAmount * 0.9 : totalAmount; //amount after discount
                            var vatAmount = totalAfterDiscount * 0.13 + totalAfterDiscount;  //amount with vat

                            var bookCar = new CustomerBooking
                            {
                                customerId = userID.ToString(),
                                CarId = model.CarId,
                                RentStartdate = model.RentStartdate,
                                RentEnddate = model.RentEnddate,
                                TotalAmount = (int)Math.Round(vatAmount)
                            };



                            var RequestInput = await _dbContext.CustomerBooking.AddAsync(bookCar);
                            await _dbContext.SaveChangesAsync(default(CancellationToken));
                            return new ResponseDTO { Status = "Success", Message = "Booking request sent" };
                        }
                    }
                    else if (role.FirstOrDefault() == "Staff")  //if user is staff
                    {

                        var customerDetails = await _dbContext.Employee.SingleOrDefaultAsync(c => c.UserId == userID.ToString());

                        if (customerDetails.IsVerified == false)
                        {
                            return new ResponseDTO { Status = "Error", Message = "Customer not verified" };
                        }
                        else
                        {
                            var rentDays = (model.RentEnddate - model.RentStartdate).TotalDays + 1;
                            var totalAmount = car.RatePerDay * rentDays;
                            var totalAfterDiscount = totalAmount * 0.75;
                            var vatAmount = totalAfterDiscount * 0.13 + totalAfterDiscount;
                            var bookCar = new CustomerBooking
                            {
                                customerId = userID.ToString(),
                                CarId = model.CarId,
                                RentStartdate = model.RentStartdate,
                                RentEnddate = model.RentEnddate,
                                TotalAmount = (int)Math.Round(vatAmount)
                            };
                            var RequestInput = await _dbContext.CustomerBooking.AddAsync(bookCar);
                            await _dbContext.SaveChangesAsync(default(CancellationToken));
                            return new ResponseDTO { Status = "Success", Message = "Booking request sent" };
                        }

                    }
                    else
                    {
                        return new ResponseDTO { Status = "error", Message = "Something went wrong" };
                    }
                }

                


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
                var baseUrl = "https://localhost:7190/images/";

                var bookingRequests = await (
                from booking in _dbContext.CustomerBooking
                where booking.isDeleted == false || booking.isDeleted == null
                let employee = _dbContext.Employee.FirstOrDefault(e => e.UserId == booking.customerId)
                let customer = _dbContext.Customer.FirstOrDefault(c => c.UserId == booking.customerId)
                let car = _dbContext.Car.FirstOrDefault(c => c.Id.ToString() == booking.CarId)
                let customerName = employee != null ? employee.Name : customer.Name
                let customerPhone = employee != null ? employee.Phone : customer.Phone

                select new GetCarBookingRequestDTO
                {
                    BookingId = booking.Id.ToString(),
                    CustomerId = booking.customerId.ToString(),
                    CustomerName = customerName,
                    CustomerPhone = customerPhone,
                    CarId = booking.CarId,
                    CarName = car.Name, 
                    Image = baseUrl + car.Image,
                    RentStartdate = booking.RentStartdate,
                    RentEnddate = booking.RentEnddate,
                    TotalAmount = booking.TotalAmount,  
                    IsAppoved = booking.IsApproved,
                    IsCompleted = booking.IsComplete,
                    OnRent = booking.OnRent,
                    payment = booking.payment,
                    IsDeleted=booking.isDeleted,
                    CreatedAt = booking.CreatedAt
                }
                ).OrderByDescending(b => b.CreatedAt).ToListAsync();

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
                var entityToUpdate = await _dbContext.CustomerBooking.FindAsync(Guid.Parse(bookingId));

                if (entityToUpdate == null)
                {
                    return new ResponseDTO() { Status = "Error", Message = "Booking not found" };
                }

                entityToUpdate.isDeleted = true; //marking as deleted
                _dbContext.CustomerBooking.Update(entityToUpdate);
                await _dbContext.SaveChangesAsync(default(CancellationToken));
                return new ResponseDTO() { Status = "Success", Message = "Booking request disapproved." };
            }
            
            catch (Exception ex) {
                return new ResponseDTO() { Status = "unsuccessful", Message = ex.Message.ToString()};
            }
        }

    }
}
