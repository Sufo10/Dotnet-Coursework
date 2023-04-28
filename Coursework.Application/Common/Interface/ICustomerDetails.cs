using System;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;

namespace Coursework.Application.Common.Interface
{
	public interface ICustomerDetails
	{
		Task<List<Customer>> GetAllCustomerService();

        Task<ResponseDTO> UploadDocument(CustomerFileUploadDTO model, string userEmail);
        Task<ResponseDataDTO<IEnumerable<BookingHistoryResponseDTO>>> GetCarHistory(string userEmail);

    }
}

