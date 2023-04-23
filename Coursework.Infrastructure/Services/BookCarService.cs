using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Infrastructure.Services
{
    public class BookCarService : IBookCar


    {
        private readonly IApplicationDBContext _dbContext;
        public BookCarService(IApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
        }


        public Task<ResponseDTO> BookCarRequest(BookCarRequestDTO model)
        {
            try {

            }
            catch (Exception ex) { }
        }
    }
}
