using System;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;

namespace Coursework.Application.Common.Interface
{
	public interface ICarBookingHistory
    {
        Task<ResponseDataDTO<IEnumerable<BookingHistoryResponseDTO>>> GetCarHistory(string id);

        //Task<ResponseDataDTO<IEnumerable<SalesRecordResponseDTO>>> GetSalesRecord(DateTime startDate, DateTime endDate);
        Task<ResponseDTO> AddAdditionalCharges(AdditionalChargeRequestDTO model);

        Task<ResponseDTO> AddAdditionalChargesUpdate(string chargeID, float amount);

        Task<ResponseDataDTO<IEnumerable<SalesRecordResponseDTO>>> GetSalesRecord();
        Task<ResponseDataDTO<IEnumerable<AdditionalChargetDTO>>> GetAdditionalCharges(string userEmail);
        Task<ResponseDataDTO<IEnumerable<AdditionalChargetDTO>>> GetAdditionalCharges2();
    }
}

