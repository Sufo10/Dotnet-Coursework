﻿using System;
using System.Net.Mail;
using System.Xml.Linq;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

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
                    UserId = new Guid(user.Id)
                };
                var customerID = customer.Id;
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var token = UtilityService.ToUrlSafeBase64(emailConfirmationToken);
                await _emailService.SendEmailConfirmationAsync(model.Name, user.Id, model.Email, token);
                if (model.File == null || model.File.Length == 0)
                {
                    _dbContext.Customer.AddAsync(customer);
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
                    customer.IsVerified = true;
                    _dbContext.Customer.AddAsync(customer);
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
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
                    if (!result.Succeeded) return new LoginResponseDTO { Status = "Error", Message = "Invalid username or password" };

                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault();
                    var token = _tokenService.GenerateToken(user, role!);
                    return new LoginResponseDTO { Status = "Success", Message = "Login Success", Data = token, Role = role };
                }
            }
            catch (Exception err)
            {
                return new LoginResponseDTO { Status = "Error", Message = err.ToString() };
            }
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var token = UtilityService.ToUrlSafeBase64(passwordResetToken);
                await _emailService.SendForgotPasswordEmailAsync(user.UserName, email, token);
            }
        }

        public async Task ResetPassword(string email, string token, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var passwordResetToken = UtilityService.FromUrlSafeBase64(token);
                var result = await _userManager.ResetPasswordAsync(user, passwordResetToken, password);
                UtilityService.ValidateIdentityResult(result);
            }
        }

        public async Task ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var emailConfirmationToken = UtilityService.FromUrlSafeBase64(token);
            var result = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);
            UtilityService.ValidateIdentityResult(result);
        }
    }
}



