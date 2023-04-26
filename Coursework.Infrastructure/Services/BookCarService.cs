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

        public BookCarService(IApplicationDBContext dBContext,  UserManager<AppUser> userManager)
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
            try {

   
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
            catch (Exception ex) {
                return new ResponseDTO { Status = "Error", Message = ex.ToString() };
            }
        }


        public async Task<ResponseDataDTO<List<GetCarBookingRequestDTO>>> GetBookCarRequests()
        {


            var bookingEmployee = _dbContext.CustomerBooking
                .Join(
                    _dbContext.Employee,
                    b => b.customerId,
                    e => e.UserId,

                    (b, e) => new GetCarBookingRequestDTO
                    {
                        BookingId = b.Id.ToString(),
                        CustomerId = b.customerId.ToString(),
                        CustomerName = e.Name,
                        CustomerPhone = e.Phone,
                        CarId = b.CarId,
                        RentStartdate = b.RentStartdate,
                        RentEnddate = b.RentEnddate,
                    }
                );



            var bookingCustomer = _dbContext.CustomerBooking
                .Join(
                    _dbContext.Customer,
                    b => b.customerId,
                    c => c.UserId,
                    (b, c) => new GetCarBookingRequestDTO
                    {
                         BookingId = b.Id.ToString(), 
                         CustomerId = b.customerId.ToString(),
                         CustomerName = c.Name,
                         CustomerPhone = c.Phone,
                         CarId = b.CarId,
                         RentStartdate = b.RentStartdate,
                         RentEnddate = b.RentEnddate,
                    }
                );

            var bookingRequests = Enumerable.Concat(bookingEmployee, bookingCustomer).ToList();

            return new ResponseDataDTO<List<GetCarBookingRequestDTO>> { Status = "Success", Message = "Data Fetched Successully", Data = bookingRequests.ToList() };


        }


    }
}
