using System;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;

namespace Coursework.Application.Common.Interface
{
	public interface ICarBookingHistory
    {
        Task<ResponseDataDTO<IEnumerable<BookingHistoryResponseDTO>>> GetCarHistory(string id);
        Task<ResponseDataDTO<IEnumerable<SalesRecordResponseDTO>>> GetSalesRecord(DateTime startDate, DateTime endDate);
    }
}

