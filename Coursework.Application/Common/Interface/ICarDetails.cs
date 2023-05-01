using System;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Coursework.Application.Common.Interface
{
	public interface ICarDetails
	{
		Task<ResponseDTO> AddCars(EditCarRequestDTO model);
		Task<ResponseDataDTO<List<CarUserDTO>>> GetActiveCars();
		Task<ResponseDataDTO<List<CarUserDTO>>> GetTrashCars();
        Task<ResponseDTO> EditCar(Guid Id, CarEditDTO model);
        Task<ResponseDTO> EditRatePerDay(Guid Id, RatePerDayDTO model);
		Task<ResponseDTO> RemoveCars(string CarId);
		Task<ResponseDTO> RestoreCar(string CarId);
    }
}

