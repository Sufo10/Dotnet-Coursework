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

        public async Task<ResponseDataDTO<List<GetIInactiveUsersDTO>>> GetInactiveUsersRequest()
        {
            var today = DateTime.Today;
            var threeMonthsAgo = today.AddMonths(-3).ToUniversalTime(); ;

            var inactiveUsers = await _dbContext.CustomerBooking
                .Where(cb => cb.RentStartdate <= threeMonthsAgo && cb.payment == true && cb.IsApproved == true)
                .ToListAsync();

            var InactiveUserDetails = new List<GetIInactiveUsersDTO>();

            foreach (var user in inactiveUsers)
            {
                var users = await _userManager.FindByIdAsync(user.customerId);
                var userRoles = await _userManager.GetRolesAsync(users);
                if (userRoles.FirstOrDefault() == "Customer")
                {
                    var customer = await _dbContext.Customer.FirstOrDefaultAsync(x => x.UserId == users.Id);

                    if (customer != null)
                    {
                        var inactivecustomerdto = new GetIInactiveUsersDTO
                        {
                            UserId = customer.UserId,
                            Name = customer.Name,
                            PhoneNumber = customer.Phone,
                        };

                        InactiveUserDetails.Add(inactivecustomerdto);

                    }
                }
                else if (userRoles.FirstOrDefault() == "Staff")
                {
                    var employee = await _dbContext.Employee.FirstOrDefaultAsync(x => x.UserId == users.Id);
                    if (employee != null)
                    {
                        var inactivecustomerdto = new GetIInactiveUsersDTO
                        {
                            UserId = employee.UserId,
                            Name = employee.Name,
                            PhoneNumber = employee.Phone,
                        };

                        InactiveUserDetails.Add(inactivecustomerdto);
                    }
                }
            }

            return new ResponseDataDTO<List<GetIInactiveUsersDTO>>
            {
                Status = "success",
                Message = "Data fetched",
                Data = InactiveUserDetails
            };



        }

        public async Task<ResponseDataDTO<List<GetMostRentalRequestDTO>>> GetMostRentalRequest()
        {
            try
            {
                var bookings = await _dbContext.CustomerBooking
                .Where(x => x.IsApproved == true && x.payment == true)
                .ToListAsync();

                var customerIds = bookings.Select(x => x.customerId).Distinct();

                var result = new List<GetMostRentalRequestDTO>();

                foreach (var customerId in customerIds)
                {
                    var count = bookings.Count(x => x.customerId == customerId);
                    var user = await _userManager.FindByIdAsync(customerId);
                    var userRoles = await _userManager.GetRolesAsync(user);

                    if (userRoles.FirstOrDefault() == "Customer")
                    {
                        var customers = await _dbContext.Customer.FirstOrDefaultAsync(x => x.UserId == customerId);
                        result.Add(new GetMostRentalRequestDTO
                        {
                            CustomerId = customers.UserId,
                            CustomerName = customers.Name,
                            NoOfRequest = count
                        });
                    }
                    else if (userRoles.FirstOrDefault() == "Staff")
                    {
                        var employees = await _dbContext.Employee.FirstOrDefaultAsync(x => x.UserId == customerId);
                        result.Add(new GetMostRentalRequestDTO
                        {
                            CustomerId = employees.UserId,
                            CustomerName = employees.Name,
                            NoOfRequest = count
                        });
                    }


                }

                var topThree = result.OrderByDescending(x => x.NoOfRequest).Take(3).ToList();
                return new ResponseDataDTO<List<GetMostRentalRequestDTO>>
                {
                    Status = "success",
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
