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
    
    public class TrackUsersServices : ITrackUsers


    {
        private readonly IApplicationDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;


        public TrackUsersServices(IApplicationDBContext dBContext, UserManager<AppUser> userManager )
        {
            _dbContext = dBContext;
            _userManager = userManager;

        }
        public async Task<ResponseDataDTO<List<GetMostRentalRequestDTO>>> GetMostRentalRequest()
        {
            try
            {
                var bookings = await _dbContext.CustomerBooking
                .Where(x => x.IsApproved == true && x.rented == true && x.payment == true)
                .ToListAsync();

                var customerIds = bookings.Select(x => x.customerId).Distinct();

                var result = new List<GetMostRentalRequestDTO>();

                foreach (var customerId in customerIds)
                {
                    var count = bookings.Count(x => x.customerId == customerId);
                    var user = await _userManager.FindByIdAsync(customerId);
                    var userRoles = await _userManager.GetRolesAsync(user);

                    if (userRoles.FirstOrDefault() == "customer")
                    {
                        var customer = await _dbContext.Customer.FirstOrDefaultAsync(x => x.UserId == customerId);
                        result.Add(new GetMostRentalRequestDTO
                        {
                            CustomerId = customer.UserId,
                            CustomerName = customer.Name,
                            NoOfRequest = count
                        });
                    }
                    else if (userRoles.FirstOrDefault() == "employee")
                    {
                        var customer = await _dbContext.Employee.FirstOrDefaultAsync(x => x.UserId == customerId);
                        result.Add(new GetMostRentalRequestDTO
                        {
                            CustomerId = customer.UserId,
                            CustomerName = customer.Name,
                            NoOfRequest = count
                        });
                    }


                }

                var topThree = result.OrderByDescending(x => x.NoOfRequest).Take(3).ToList();
                return new ResponseDataDTO<List<GetMostRentalRequestDTO>>
                {
                    Status = "successful",
                    Message = "fetch successful",
                    Data = topThree
                };
            }

            catch (Exception ex)
            {
                return new ResponseDataDTO<List<GetMostRentalRequestDTO>>
                {
                    Status = "error",
                    Message = ex.Message.ToString(),   
                };
            }

        }
    }
}
