﻿using System;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Coursework.Infrastructure.Services
{
	public class CarService:ICarDetails
	{
        private readonly IApplicationDBContext _dbContext;
        private readonly IFileStorage _fileStorage;
        public CarService(IApplicationDBContext dBContext, IFileStorage fileStorage)
        {
            _dbContext = dBContext;
            _fileStorage = fileStorage;
        }
        public async Task<string> UploadAsync(IFormFile file)
        {
            var fileName = file.FileName;

            if (file.Length > 1 * 1024 * 1024) // 1MB
                throw new Exception("File size exceeds the limit");

            return await _fileStorage.SaveFileAsync(file);
        }


        public async Task<ResponseDTO> AddCars(NewCarRegisterRequestDTO model)
        {
            try
            {
                var uploadedFile = await UploadAsync(model.File);
                var newCar = new Car
                {
                    Name = model.Name,
                    IsAvailable = true,
                    RatePerDay = model.RatePerDay,
                    Image = uploadedFile,
                    Description=model.Description,

                };
                _dbContext.Car.AddAsync(newCar);
                await _dbContext.SaveChangesAsync(default(CancellationToken));
                return new ResponseDTO { Status = "Success", Message = "Car Created successfully" };

            }
            catch (Exception err)
            {
                return new ResponseDTO { Status = "Error", Message = err.ToString() };

            }
        }
        public async Task<ResponseDataDTO> GetActiveCars()
        {
            var data = _dbContext.Car.Select(e => new Car()
            {
                Id = e.Id,
                Name=e.Name,
                Image=e.Image,
                IsAvailable=e.IsAvailable
            }).ToList();

            return new ResponseDataDTO { Status="Success",Message="Data Fetched Successully",Data=data};
        }
    }
}

