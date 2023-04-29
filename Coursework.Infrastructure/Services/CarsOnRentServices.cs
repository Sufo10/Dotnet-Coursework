using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Infrastructure.Services
{
    internal class CarsOnRentServices : ICarsOnRent
    {
        private readonly IApplicationDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;

        public CarsOnRentServices(IApplicationDBContext dBContext, UserManager<AppUser> userManager, IEmailService emailService)
        {
            _dbContext = dBContext;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<ResponseDTO> RemoveRent(string BookingId)
        {
            try
            {
                var entityToUpdate = await _dbContext.CustomerBooking.FindAsync(Guid.Parse(BookingId));

                if (entityToUpdate == null)
                {
                    return new ResponseDTO() { Status = "Error", Message = "Booking not found" };
                }

                entityToUpdate.OnRent = false;
                _dbContext.CustomerBooking.Update(entityToUpdate);
                await _dbContext.SaveChangesAsync(default(CancellationToken));
                return new ResponseDTO() { Status = "Success", Message = "Car is now returned" };
            }

            catch (Exception ex)
            {
                return new ResponseDTO() { Status = "error", Message = ex.Message.ToString() };
            }

        }

        public async Task<ResponseDTO> RentCars(string BookingId)
        {
            try
            {
                var booking = await _dbContext.CustomerBooking.FindAsync(Guid.Parse(BookingId));

                if (booking == null)
                    return new ResponseDTO { Status = "Error", Message = "Booking not found" };

                if (booking.OnRent != true || booking.IsComplete != false)
                    return new ResponseDTO { Status = "Error", Message = "Car is already on rent or completed" };

                booking.OnRent = true;
                booking.IsComplete = true;
                await _dbContext.SaveChangesAsync(default(CancellationToken));
                return new ResponseDTO { Status = "Success", Message = "Car is now on rent" };
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Status = "Error", Message = ex.Message };
            }
        }

    }
}
