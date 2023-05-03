using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coursework.API.Controllers
{

    [ApiController]
    public class TrackUsersController : ControllerBase
    {
        private readonly ITrackUsers _Track;

        public TrackUsersController(ITrackUsers track)
        {
            _Track = track;
        }

        [Authorize (Roles = "Staff,Admin")]
        [HttpGet]
        [Route("/api/mostRequest")]
        public async Task<ResponseDataDTO<List<GetMostRentalRequestDTO>>> GetMostRentalRequesttt()
        {
            var data = await _Track.GetMostRentalRequest();
            return data;
        }

        [Authorize (Roles = "Staff,Admin")]
        [HttpGet]
        [Route("/api/inactiveUsers")]

        public async Task<ResponseDataDTO<List<GetIInactiveUsersDTO>>> GetInactiveUsersRequesttt()
        {
            var data = await _Track.GetInactiveUsersRequest();
            return data;
        }
    }
}
