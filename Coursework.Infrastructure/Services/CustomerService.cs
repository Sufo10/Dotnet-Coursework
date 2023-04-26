﻿using System;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Coursework.Infrastructure.Services
{
    public class CustomerService : ICustomerDetails
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IApplicationDBContext _dbContext;
        private readonly IFileStorage _fileStorage;

        public CustomerService(UserManager<AppUser> userManager, IApplicationDBContext dBContext, IFileStorage fileStorage)
        {
            _userManager = userManager;
            _dbContext = dBContext;
            _fileStorage = fileStorage;
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var fileName = file.FileName;

            if (!IsFileExtensionValid(fileName))
                throw new Exception("Only pdf and png is supported");

            if (file.Length > 1.5 * 1024 * 1024) // 1MB
                throw new Exception("File size exceeds the limit");

            return await _fileStorage.SaveFileAsync(file);
        }

        private bool IsFileExtensionValid(string fileName)
        {
            var validExtensions = new[] { ".pdf", ".png" };
            var fileExtension = Path.GetExtension(fileName);

            return validExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<List<Customer>> GetAllCustomerService()
        {
            var data = _dbContext.Customer.Select(e => new Customer()
            {
                Id = e.Id,
                Name = e.Name,
                CustomerType = e.CustomerType
            }).ToList();
            return data;
        }

        public async Task<ResponseDTO> UploadDocument(CustomerFileUploadDTO model, Guid userID)
        {
            try
            {
                var customerDetails = await _dbContext.Customer.SingleOrDefaultAsync(c => c.UserId == userID.ToString());

                if (customerDetails == null)
                {
                    return new ResponseDTO { Status = "Error", Message = "Customer not found." };
                }

                if (customerDetails.IsVerified == true)
                {
                    return new ResponseDTO { Status = "Error", Message = "Customer is already verified" };
                }
                else
                {
                    var uploadedFile = await UploadAsync(model.File);
                    var customerUpload = new CustomerFileUpload
                    {
                        FileName = uploadedFile,
                        UserId = userID.ToString(),
                        DocumentType = model.FileType,
                        CreatedBy = customerDetails.Id
                    };
                    customerDetails.IsVerified = true;
                    var customerInput = await _dbContext.CustomerFileUpload.AddAsync(customerUpload);
                    _dbContext.Customer.Update(customerDetails);
                    //if(customerInput)
                    await _dbContext.SaveChangesAsync(default(CancellationToken));
                    return new ResponseDTO { Status = "Success", Message = "Document added successfully." };
                }

            }
            catch (Exception err)
            {
                return new ResponseDTO { Status = "Error", Message = err.Message.ToString() };
            }
        }


    }
}

