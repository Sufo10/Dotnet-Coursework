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
                .Where(cb => cb.RentEnddate <= threeMonthsAgo && cb.OnRent == true && cb.payment == true && cb.IsApproved == true)
                .ToListAsync();

            var inactiveUsersDTOs = new List<GetIInactiveUsersDTO>();
            foreach (var user in inactiveUsers)
            {
                if (user.customerId != null)
                {
                    var customer = await _dbContext.Customer.FindAsync(user.customerId);
                    if (customer != null)
                    {
                        var inactiveCustomerDTO = new GetIInactiveUsersDTO
                        {
                            UserId = customer.UserId,
                            Name = customer.Name,
                            PhoneNumber = customer.Phone,
                        };
                        inactiveUsersDTOs.Add(inactiveCustomerDTO);
                    }
                }
                else if (user.ApprovedBy != null)
                {
                    var employee = await _dbContext.Employee.FindAsync(user.ApprovedBy);
                    if (employee != null)
                    {
                        var inactiveEmployeeDTO = new GetIInactiveUsersDTO
                        {
                            UserId = employee.UserId,
                            Name = employee.Name,
                            PhoneNumber = employee.Phone,
                        };
                        inactiveUsersDTOs.Add(inactiveEmployeeDTO);
                    }
                }
            }

            return new ResponseDataDTO<List<GetIInactiveUsersDTO>>
            {
                Status = "successful",
                Message = "Data fetched",
                Data = inactiveUsersDTOs
            };



        }

        public async Task<ResponseDataDTO<List<GetMostRentalRequestDTO>>> GetMostRentalRequest()
        {
            try
            {
                var bookings = await _dbContext.CustomerBooking
                .Where(x => x.IsApproved == true && x.OnRent == true && x.payment == true)
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
