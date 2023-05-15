using System;
using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Coursework.Infrastructure.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace Coursework.Infrastructure.Services
{
	public class CarService:ICarDetails
	{
        private readonly IApplicationDBContext _dbContext;
        private readonly IFileStorage _fileStorage;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;

        public CarService(IApplicationDBContext dBContext, IFileStorage fileStorage, UserManager<AppUser> userManager, IEmailService emailService)
        {
            _dbContext = dBContext;
            _fileStorage = fileStorage;
            _userManager = userManager;
            _emailService = emailService;
        }
        public async Task<string> UploadAsync(IFormFile file)
        {
            var fileName = file.FileName;

            if (file.Length > 1 * 1024 * 1024) // 1.5MB
                throw new Exception("File size exceeds the limit");

            return await _fileStorage.SaveFileAsync(file);
        }


        public async Task<ResponseDTO> AddCars(EditCarRequestDTO model)
        {
            try
            {
                var uploadedFile = await UploadAsync(model.File);
                var newCar = new Car //new car object
                {
                    Name = model.Name,
                    IsAvailable = true,
                    RatePerDay = model.RatePerDay == 0 ? model.ActualPrice : model.RatePerDay,
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
            var data = _dbContext.Car
                .Where(c => c.isDeleted != true)
                .Select(e => new CarUserDTO()
            {
                Id = e.Id,
                Name=e.Name,
                Image = baseUrl + e.Image, //adding image with base url
                IsAvailable=e.IsAvailable,
                Description=e.Description,
                RatePerDay=e.RatePerDay,
                ActualPrice=e.ActualPrice
                
            }).ToList();

            return new ResponseDataDTO<List<CarUserDTO>> { Status="Success",Message="Data Fetched Successully",Data=data};
        }

        public async Task<ResponseDataDTO<List<CarUserDTO>>> GetTrashCars()


        {
            var baseUrl = "https://localhost:7190/images/";
            var data = _dbContext.Car
                .Where(c => c.isDeleted == true)
                .Select(e => new CarUserDTO()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Image = baseUrl + e.Image,  //adding image with base url
                    IsAvailable = e.IsAvailable,
                    Description = e.Description,
                    RatePerDay = e.RatePerDay,
                    ActualPrice = e.ActualPrice

                }).ToList();

            return new ResponseDataDTO<List<CarUserDTO>> { Status = "Success", Message = "Data Fetched Successully", Data = data };
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
                if (model.RatePerDay > car.RatePerDay)
                {

                // Update the car properties with values from the CarEditDTO object
                car.Name = model.Name;
                car.ActualPrice = model.RatePerDay; 
                car.Description = model.Description;

                // Save the changes to the database
                await _dbContext.SaveChangesAsync(default(CancellationToken));

                return new ResponseDTO { Status = "Success", Message = "Car Edited successfully" };
                }
                else
                {
                    return new ResponseDTO { Status = "Error", Message = "Offer Price is set higher than your actual price." };

                }

            }
            catch (Exception err)
            {
                return new ResponseDTO { Status = "Error", Message = err.ToString() };
            }
        }


        public async Task<ResponseDTO> EditRatePerDay(Guid Id, RatePerDayDTO model)
        {
            try
            {
                var car = await _dbContext.Car.FindAsync(Id);

                var users = await _userManager.Users.ToListAsync();

                if (car == null)
                {
                    return new ResponseDTO { Status = "Error", Message = "Rate per Day not found" };
                }

                // Validate offer price
                if (model.RatePerDay >= car.ActualPrice)
                {
                    return new ResponseDTO { Status = "Error", Message = "Offer price should be less than actual price." };
                }

                car.RatePerDay = model.RatePerDay;

                await _dbContext.SaveChangesAsync(default(CancellationToken));

                foreach (var user in users)
                {
                    var notice = new OfferNoticeDTO
                    {
                        OfferPrice = model.RatePerDay,
                        ActualAmount = car.ActualPrice,
                        Message = $"{car.Name} is on offer right now.",
                        carName = car.Name,
                        CustomerEmail = await _userManager.GetEmailAsync(user)
                    };

                    await _emailService.SendOfferNoticeAsync(notice);  //sending email about the offer
                }

                return new ResponseDTO { Status = "Success", Message = "Rate per Day Edited successfully" };
            }
            catch (Exception err)
            {
                return new ResponseDTO { Status = "Error", Message = err.ToString() };
            }
        }

        public async Task<ResponseDTO> RemoveCars(string CarId)
        {
            try
            {
                var entityToUpdate = await _dbContext.Car.FindAsync(Guid.Parse(CarId));


                if (entityToUpdate == null)
                {
                    return new ResponseDTO() { Status = "Error", Message = "Car not found" };
                }


                entityToUpdate.isDeleted = true;  //marking as deleted
                _dbContext.Car.Update(entityToUpdate);
                await _dbContext.SaveChangesAsync(default(CancellationToken));
                return new ResponseDTO() { Status = "Success", Message = "Car is removed." };
            }

            catch (Exception ex)
            {
                return new ResponseDTO() { Status = "Error", Message = ex.Message.ToString() };
            }
        }

            public async Task<ResponseDTO> RestoreCar(string CarId)
            {
                try
                {
                    var entityToUpdate = await _dbContext.Car.FindAsync(Guid.Parse(CarId));


                    if (entityToUpdate == null)
                    {
                        return new ResponseDTO() { Status = "Error", Message = "Car not found" };
                    }


                    entityToUpdate.isDeleted = false;  // marking as not deleted
                    _dbContext.Car.Update(entityToUpdate);
                    await _dbContext.SaveChangesAsync(default(CancellationToken));
                    return new ResponseDTO() { Status = "Success", Message = "Car is restored." };
                }

                catch (Exception ex)
                {
                    return new ResponseDTO() { Status = "Error", Message = ex.Message.ToString() };
                }
            }
        }
    }

