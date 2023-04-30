using System;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;

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


        public async Task<ResponseDTO> AddCars(EditCarRequestDTO model)
        {
            try
            {
                var uploadedFile = await UploadAsync(model.File);
                var newCar = new Car
                {
                    Name = model.Name,
                    IsAvailable = true,
                    RatePerDay = model.RatePerDay ?? model.ActualPrice,
                    Image = uploadedFile,
                    Description=model.Description,
                    ActualPrice=model.ActualPrice
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
        public async Task<ResponseDataDTO<List<CarUserDTO>>> GetActiveCars()
        {
             var baseUrl = "https://localhost:7190/images/";
            var data = _dbContext.Car.Select(e => new CarUserDTO()
            {
                Id = e.Id,
                Name=e.Name,
                Image = baseUrl + e.Image,
                IsAvailable=e.IsAvailable,
                Description=e.Description,
                RatePerDay=e.RatePerDay,
                ActualPrice=e.ActualPrice
                
            }).ToList();

            return new ResponseDataDTO<List<CarUserDTO>> { Status="Success",Message="Data Fetched Successully",Data=data};
        }

        public async Task<ResponseDTO> EditCar(Guid Id, CarEditDTO model)
        {
            try
            {
                // Retrieve the car from the database
                var car = await _dbContext.Car.FindAsync(Id);

                if (car == null)
                {
                    // If car not found, return an error response
                    return new ResponseDTO { Status = "Error", Message = "Car not found" };
                }

                // Update the car properties with values from the CarEditDTO object
                car.Name = model.Name;
                car.ActualPrice = model.RatePerDay;
                car.Description = model.Description;

                // Save the changes to the database
                await _dbContext.SaveChangesAsync(default(CancellationToken));

                return new ResponseDTO { Status = "Success", Message = "Car Edited successfully" };
            }
            catch (Exception err)
            {
                return new ResponseDTO { Status = "Error", Message = err.ToString() };
            }
        }
    }
}

