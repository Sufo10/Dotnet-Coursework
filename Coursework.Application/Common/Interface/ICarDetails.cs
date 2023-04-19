using System;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;

namespace Coursework.Application.Common.Interface
{
	public interface ICarDetails
	{
		Task<ResponseDTO> AddCars(NewCarRegisterRequestDTO model);
		Task<ResponseDataDTO> GetActiveCars();
	}
}

