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

        public async Task<ResponseDTO> ApproveBookingRequest(BookingApproveRequestDTO model, Guid userID)
        {
            var entityToUpdate = await _dbContext.CustomerBooking.FindAsync(userID.ToString());

            if (entityToUpdate == null)
            {
                return new ResponseDTO() { Status = "Error", Message = "Entity not found" };
            }

            entityToUpdate.ApprovedBy = userID.ToString();
            entityToUpdate.IsApproved = true;

            _dbContext.CustomerBooking.Update(entityToUpdate);
            await _dbContext.SaveChangesAsync(default(CancellationToken));

            return new ResponseDTO() { Status = "Success", Message = "Booking request approved" };
        }

        public async Task<ResponseDTO> BookCarRequest(BookCarRequestDTO model, Guid userID)
        {
            try
            {
                //var customerDetails = await _dbContext.Customer.SingleOrDefaultAsync(c => c.UserId == userID.ToString());

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


        //public async Task<ResponseDataDTO<List<GetCarBookingRequestDTO>>> GetBookCarRequests()
        //{


        //    var bookingEmployee = _dbContext.CustomerBooking
        //        .Join(
        //            _dbContext.Employee,
        //            b => b.customerId,
        //            e => e.UserId,

        //            (b, e) => new { Booking = b, Employee = e }
        //            )
        //             .Where(x => x.Booking.isDeleted == false || x.Booking.isDeleted == null)
        //             .Select(x => new GetCarBookingRequestDTO
        //             {

        //                 BookingId = x.Booking.Id.ToString(),
        //                 CustomerId = x.Booking.customerId.ToString(),
        //                 CustomerName = x.Employee.Name,
        //                 CustomerPhone = x.Employee.Phone,
        //                 CarId = x.Booking.CarId,
        //                 RentStartdate = x.Booking.RentStartdate,
        //                 RentEnddate = x.Booking.RentEnddate,
        //             });



        //    var bookingCustomer = _dbContext.CustomerBooking
        //            .Join(
        //                _dbContext.Customer,
        //                b => b.customerId,
        //                c => c.UserId,
        //                (b, c) => new { Booking = b, Customer = c }
        //            )
        //            .Where(x => x.Booking.isDeleted == false || x.Booking.isDeleted == null)
        //            .Select(x => new GetCarBookingRequestDTO
        //            {
        //                BookingId = x.Booking.Id.ToString(),
        //                CustomerId = x.Booking.customerId.ToString(),
        //                CustomerName = x.Customer.Name,
        //                CustomerPhone = x.Customer.Phone,
        //                CarId = x.Booking.CarId,
        //                RentStartdate = x.Booking.RentStartdate,
        //                RentEnddate = x.Booking.RentEnddate,
        //            });

        //    var bookingRequests = Enumerable.Concat(bookingEmployee, bookingCustomer).ToList();

        //    return new ResponseDataDTO<List<GetCarBookingRequestDTO>> { Status = "Success", Message = "Data Fetched Successully", Data = bookingRequests.ToList() };
        //}

        public async Task<ResponseDataDTO<List<GetCarBookingRequestDTO>>> GetBookCarRequests()
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


        public async Task<ResponseDTO> RejectBookingRequest(string bookingId)
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

    }
}
