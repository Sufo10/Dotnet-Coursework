using System;
using System.Net.Mail;
using System.Security.Claims;
using System.Transactions;
using System.Xml.Linq;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Coursework.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticate
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IApplicationDBContext _dbContext;
        private readonly IFileStorage _fileStorage;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        public AuthenticationService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, IApplicationDBContext dBContext, IFileStorage fileStorage, ITokenService tokenService, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _dbContext = dBContext;
            _fileStorage = fileStorage;
            _tokenService = tokenService;
            _emailService = emailService;
        }
        public async Task<string> UploadAsync(IFormFile file)
        {
            var fileName = file.FileName;

            if (!IsFileExtensionValid(fileName))
                throw new Exception("Only pdf and png is supported");

            if (file.Length > 1.5 * 1024 * 1024) // 1MB
                throw new Exception("File size exceeds the limit. Only file upto 1.5MB is supported");

            return await _fileStorage.SaveFileAsync(file);
        }

        private bool IsFileExtensionValid(string fileName)
        {
            var validExtensions = new[] { ".pdf", ".png" };
            var fileExtension = Path.GetExtension(fileName);

            return validExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
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
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                AppUser user = new()
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
                    UserId = user.Id
                };
                var customerID = customer.Id;
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var token = UtilityService.ToUrlSafeBase64(emailConfirmationToken);
                await _emailService.SendEmailConfirmationAsync(model.Name, user.Id, model.Email, token);
                if (model.File == null || model.File.Length == 0)
                {
                    _dbContext.Customer.AddAsync(customer);
                    await _dbContext.SaveChangesAsync(default(CancellationToken));
                    scope.Complete();
                    return new ResponseDTO { Status = "Success", Message = "Customer Created successfully" };
                }
                else
                {
                    var uploadedFile = await UploadAsync(model.File);
                    var customerUpload = new CustomerFileUpload
                    {
                        FileName = uploadedFile,
                        UserId = user.Id,
                        DocumentType = model.FileType,
                        CreatedBy = customerID
                    };
                    customer.IsVerified = true;
                    _dbContext.Customer.AddAsync(customer);

                    var customerInput = await _dbContext.CustomerFileUpload.AddAsync(customerUpload);
                    //if(customerInput)
                    await _dbContext.SaveChangesAsync(default(CancellationToken));

                    scope.Complete();
                    return new ResponseDTO { Status = "Success", Message = "Customer Created successfully" };
                }
            }
            catch (Exception err)
            {
                return new ResponseDTO { Status = "Error", Message = err.Message.ToString() };
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
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault();
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                    if (!result.Succeeded) return new LoginResponseDTO { Status = "Error", Message = "Invalid username or password" };
                    var token = _tokenService.GenerateToken(user, role);
                    if (role == "Customer")
                    {
                        var customer = await _dbContext.Customer.SingleOrDefaultAsync(c => c.UserId == user.Id.ToString());
                        return new LoginResponseDTO { Status = "Success", Message = "Login Success", Data = token, Role = role, UserName = model.UserName, IsVerified = customer.IsVerified };
                    }
                    return new LoginResponseDTO { Status = "Success", Message = "Login Success", Data = token, Role = role, UserName = model.UserName };
                }
            }
            catch (Exception err)
            {
                return new LoginResponseDTO { Status = "Error", Message = err.ToString() };
            }
        }

        public async Task<ResponseDTO> ForgotPasswordEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var token = UtilityService.ToUrlSafeBase64(passwordResetToken);
                await _emailService.SendForgotPasswordEmailAsync(user.UserName, email, token);
                return new ResponseDTO { Status = "Success", Message = "Email Sent Successful" };
            }
            else
            {
                return new ResponseDTO { Status = "Error", Message = "User Not Found" };
            }
        }

        public async Task<ResponseDTO> ResetPasswordAsync(ResetPasswordRequestDTO body)
        {
            var user = await _userManager.FindByEmailAsync(body.Email);
            if (user != null)
            {
                var passwordResetToken = UtilityService.FromUrlSafeBase64(body.Token!);
                var currentPassword = await _userManager.CheckPasswordAsync(user, body.Password);
                if (currentPassword)
                {
                    return new ResponseDTO { Status = "Error", Message = "New password cannot match current password" };
                }
                var result = await _userManager.ResetPasswordAsync(user, passwordResetToken, body.Password);
                UtilityService.ValidateIdentityResult(result);
                return new ResponseDTO { Status = "Success", Message = "Password Reset Successful" };
            }
            else
            {
                return new ResponseDTO { Status = "Error", Message = "User Not Found" };
            }

        }

        public async Task ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var emailConfirmationToken = UtilityService.FromUrlSafeBase64(token);
            var result = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);
            UtilityService.ValidateIdentityResult(result);
        }

        public async Task<ResponseDTO> EmployeeRegister(EmployeeRegistrationRequestDTO model, string userID)
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
                AppUser user = new()
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

                if (model.EmployeeType.ToUpper() == "ADMIN" || model.EmployeeType.ToUpper() == "STAFF")
                {

                    var roleExists = await _roleManager.RoleExistsAsync(model.EmployeeType);
                    if (!roleExists)
                    {
                        await _roleManager.CreateAsync(new IdentityRole { Name = model.EmployeeType });
                    }

                    await _userManager.AddToRoleAsync(user, model.EmployeeType);
                    var employee = new CompanyEmployee
                    {
                        Name = model.Name,
                        Address = model.Address,
                        IsVerified = false,
                        Phone = model.Phone,
                        UserId = user.Id,
                        PaymentFulfilled = true,

                    };
                    var employeeID = employee.Id;
                    _dbContext.Employee.AddAsync(employee);
                    await _dbContext.SaveChangesAsync(default(CancellationToken));
                    return new ResponseDTO { Status = "Success", Message = "Employee Created successfully" };
                }
                else
                {
                    return new ResponseDTO { Status = "Error", Message = "Employee Type can only be admin or staff" };

                }
            }
            catch (Exception err)
            {
                return new ResponseDTO { Status = "Error", Message = err.ToString() };
            }
        }

        public async Task<ResponseDTO> ChangePassword(UserChangePasswordDTO model,string email)
        {
            try
            {
                var currentUser = await _userManager.FindByEmailAsync(email);

                var passwordIsValid = await _userManager.CheckPasswordAsync(currentUser, model.Password);
                if (!passwordIsValid)
                {
                    return new ResponseDTO { Status = "Error", Message = "Invalid current password" };
                }

                if (model.Password.Equals(model.NewPassword))
                {
                    return new ResponseDTO { Status = "Error", Message = "New Password cannot be same as current password" };

                }

                if (!model.NewPassword.Equals(model.ConfirmPassword))
                {
                    return new ResponseDTO { Status = "Error", Message = "Confirm Password doesnot match" };
                }

                // Attempt to change the password
                var result = await _userManager.ChangePasswordAsync(currentUser, model.Password, model.NewPassword);
                if (!result.Succeeded)
                {
                    return new ResponseDTO { Status = "Error", Message = $"{result.Errors.FirstOrDefault()?.Description}" };
                }

                return new ResponseDTO { Status = "Success", Message = "Password Changed Successfully!" };

            }
            catch (Exception ex)
            {
                return new ResponseDTO { Status = "Error", Message = "Some Problem Occured" };

            }
        }


        public async Task<ResponseDataDTO<List<EmployeeResponseDTO>>> GetAllEmployee()
        {
            var employees = await _dbContext.Employee
        .Include(e => e.User)
        .ToListAsync();

            var employeeDtos = new List<EmployeeResponseDTO>();

            foreach (var employee in employees)
            {
                var user = await _userManager.FindByIdAsync(employee.UserId);

                var role = await _userManager.GetRolesAsync(user);
                var roleName = role.FirstOrDefault();

                employeeDtos.Add(new EmployeeResponseDTO
                {
                    Id = employee.Id.ToString(),
                    Name = employee.Name,
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = roleName
                });
            }

            return new ResponseDataDTO<List<EmployeeResponseDTO>> { Status = "Success", Message = "Data Fetch Succesfully", Data = employeeDtos };
        }


    }
}



