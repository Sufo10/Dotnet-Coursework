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
    public class BookCarService : IBookCar


    {
        private readonly IApplicationDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public BookCarService(IApplicationDBContext dBContext,  UserManager<AppUser> userManager)
        {
            _dbContext = dBContext;
            _userManager = userManager;
        }


        public async Task<ResponseDTO> BookCarRequest(BookCarRequestDTO model,string email)
        {
            try {
                var user =await _userManager.FindByEmailAsync(email);
                var userID = user.Id;
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
                    Console.WriteLine();
                    return new ResponseDTO { Status = "Success", Message = "Booking request sent" };
                }

            }
            catch (Exception ex) {
                return new ResponseDTO { Status = "Error", Message = ex.ToString() };
            }
        }
    }
}
