﻿using Coursework.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Application.Common.Interface
{
    public interface ICarRent
    {

        Task<ResponseDataDTO<List<GetCarBookingRequestDTO>>> GetPaidCars();

    }
}
