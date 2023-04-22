using System;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;

namespace Coursework.Application.Common.Interface
{
	public interface ICarTestDetails
	{
        Task<ResponseDataDTO<List<CarTestDTO>>> GetCarTest();
    }
}

