using System;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Coursework.Infrastructure.Services
{
	public class AuthenticationService: IAuthenticate
	{
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IApplicationDBContext _dbContext;
        public AuthenticationService(UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager, IApplicationDBContext dBContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dBContext;

        }

        public async Task<ResponseDTO> Register(CustomerRegisterRequestDTO model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return new ResponseDTO { Status = "Error", Message = "User already exists" };

            IdentityUser user = new()
            {
                Email = model.Email,
                UserName=model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return
                    new ResponseDTO
                    { Status = "Error", Message = "User Creation failed! Please check" };

            var roleExists = await _roleManager.RoleExistsAsync("Customer");
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "Customer" });
            }

            await _userManager.AddToRoleAsync(user, "Customer");
            var customer = new Customer
            {
                Name = model.Name,
                CustomerType = Domain.Models.CustomerType.Basic,
                Address = "",
                IsVerified = true,
                UserId = new Guid(user.Id)

            };
            _dbContext.Customer.AddAsync(customer);
            await _dbContext.SaveChangesAsync(default(CancellationToken));
            return new ResponseDTO { Status = "Success", Message = "Employee Created successfully" };
        }



    }
}

