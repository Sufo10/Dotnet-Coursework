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

namespace Coursework.Infrastructure.Services
{
    public class RentCars : ICarRent
    {
        private readonly IApplicationDBContext _dbContext;

        public RentCars(IApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<ResponseDataDTO<List<GetCarBookingRequestDTO>>> GetPaidCars()
        {

            // query to get all paid bookings
            var paidBookings = await (
                from booking in _dbContext.CustomerBooking
                where booking.payment == true // only include bookings that have been paid for
                let employee = _dbContext.Employee.FirstOrDefault(e => e.UserId == booking.customerId) // if the customer is also an employee, get their name and phone from the employee table
                let customer = _dbContext.Customer.FirstOrDefault(c => c.UserId == booking.customerId) // if the customer is not an employee, get their name and phone from the customer table
                let car = _dbContext.Car.FirstOrDefault(c => c.Id.ToString() == booking.CarId) // get the car associated with the booking

                // determine whether the customer is an employee or not, and use the appropriate name and phone
                let customerName = employee != null ? employee.Name : customer.Name
                let customerPhone = employee != null ? employee.Phone : customer.Phone

                // create a new object with the relevant booking information
                select new GetCarBookingRequestDTO
                {
                    BookingId = booking.Id.ToString(),
                    CustomerId = booking.customerId.ToString(),
                    CustomerName = customerName,
                    CustomerPhone = customerPhone,
                    CarId = booking.CarId,
                    CarName = car.Name,
                    Image = car.Image,
                    RentStartdate = booking.RentStartdate,
                    RentEnddate = booking.RentEnddate,
                }
            ).ToListAsync();

            // return the list of paid bookings in a response object
            return new ResponseDataDTO<List<GetCarBookingRequestDTO>> { Status = "success", Message = "data retrived", Data = paidBookings.ToList() };
        }

    }
}
