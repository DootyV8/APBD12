using APBD12.Services;
using Microsoft.AspNetCore.Mvc;
using APBD12.Dtos;

namespace APBD12.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;
        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public IActionResult GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _tripService.GetTrips(page, pageSize);
            return Ok(result);
        }

        [HttpPost("{idTrip}/clients")]
        public async Task<IActionResult> AssignClientToTrip(int idTrip, [FromBody] AssignClientDto dto)
        {
            var result = await _tripService.AssignClientToTripAsync(idTrip, dto, HttpContext.RequestAborted);
            if (!result.Success) return BadRequest(result.Message);
            return Ok();
        }
    }
}