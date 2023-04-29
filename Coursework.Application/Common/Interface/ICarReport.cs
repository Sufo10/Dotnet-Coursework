using System;
using Coursework.Application.DTO;

namespace Coursework.Application.Common.Interface
{
	public interface ICarReport
	{
        Task<ResponseDataDTO<List<CarReportDTO>>> GetActiveCars();
    }
}

