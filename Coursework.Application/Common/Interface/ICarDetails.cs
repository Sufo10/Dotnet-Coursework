using System;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;

namespace Coursework.Application.Common.Interface
{
	public interface ICarDetails
	{
		Task<ResponseDTO> AddCars(EditCarRequestDTO model);
		Task<ResponseDataDTO<List<CarUserDTO>>> GetActiveCars();
	}
}

