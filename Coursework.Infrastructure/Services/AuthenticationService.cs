using System;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Coursework.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticate
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IApplicationDBContext _dbContext;
        private readonly IFileStorage _fileStorage;
        private readonly ITokenService _tokenService;
        public AuthenticationService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, IApplicationDBContext dBContext, IFileStorage fileStorage, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _dbContext = dBContext;
            _fileStorage = fileStorage;
            _tokenService = tokenService;
        }
        public async Task<string> UploadAsync(IFormFile file)
        {
            var fileName = file.FileName;

            if (file.Length > 1 * 1024 * 1024) // 1MB
                throw new Exception("File size exceeds the limit");

            return await _fileStorage.SaveFileAsync(file);
        }


        public async Task<ResponseDTO> Register(CustomerRegisterRequestDTO model)
        {
            try
            {
                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                    return new ResponseDTO { Status = "Error", Message = "Email already exists" };

                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    return new ResponseDTO { Status = "Error", Message = "Password doesnot match" };
                }
                IdentityUser user = new()
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    var errorMessages = result.Errors.Select(e => e.Description).ToList();
                    return new ResponseDTO { Status = "Error", Message = string.Join("; ", errorMessages) };
                }

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
                    Address = model.Address,
                    IsVerified = false,
                    Phone = model.Phone,
                    UserId = new Guid(user.Id)
                };
                _dbContext.Customer.AddAsync(customer);
                var customerID = customer.Id;
                if (model.File == null || model.File.Length == 0)
                {
                    await _dbContext.SaveChangesAsync(default(CancellationToken));
                    return new ResponseDTO { Status = "Success", Message = "Customer Created successfully" };
                }
                else
                {
                    var uploadedFile = await UploadAsync(model.File);
                    var customerUpload = new CustomerFileUpload
                    {
                        FileName = uploadedFile,
                        UserID = customerID,
                        DocumentType = model.FileType,
                        CreatedBy = customerID
                    };
                    var customerInput = await _dbContext.CustomerFileUpload.AddAsync(customerUpload);
                    //if(customerInput)
                    await _dbContext.SaveChangesAsync(default(CancellationToken));
                    return new ResponseDTO { Status = "Success", Message = "Customer Created successfully" };
                }
            }
            catch (Exception err)
            {
                return new ResponseDTO { Status = "Error", Message = err.ToString() };
            }

        }

        public async Task<LoginResponseDTO> TokenLoginAsync(LoginRequestDTO model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null) { return new LoginResponseDTO { Status = "Error", Message = "Invalid username or password" }; }
                else
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (!result.Succeeded) return new LoginResponseDTO { Status = "Error", Message = "Invalid username or password" };

                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault();
                    var token =  _tokenService.GenerateToken(user, role!);
                    return new LoginResponseDTO { Status = "Success", Message = "Login Success", Data = token, Role = role };
                }
            }
            catch (Exception err)
            {
                return new LoginResponseDTO { Status = "Error", Message = err.ToString() };
            }
        }
    }
}



