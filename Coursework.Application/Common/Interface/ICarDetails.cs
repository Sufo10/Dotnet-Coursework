using System;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;

namespace Coursework.Application.Common.Interface
{
	public interface ICarDetails
	{
		Task<ResponseDTO> AddCars(EditCarRequestDTO model);
		Task<ResponseDataDTO<List<CarUserDTO>>> GetActiveCars();
		Task<ResponseDTO> EditCar(Guid Id, CarEditDTO model);
        Task<ResponseDTO> EditRatePerDay(Guid Id, RatePerDayDTO model);
    }
}

