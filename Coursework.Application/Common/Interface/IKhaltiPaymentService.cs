﻿using Coursework.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.Common.Interface
{
    public interface IKhaltiPaymentService
    {
        public Task<ResponseDataDTO<KhaltiResponseDTO>> InitializePayment(KhaltiPaymentDTO model, string email);
        public Task<ResponseDataDTO<KhaltiResponseDTO>> CheckPaymentSuccess(string pidx, string bookingId, int amount);
        public Task<ResponseDataDTO<KhaltiResponseDTO>> OfflinePayment(KhaltiPaymentDTO model);
        public Task<ResponseDataDTO<KhaltiResponseDTO>> OfflinePaymentForAdditionalCharges(AdditonalChargePaymentDTO model);

    }
}
