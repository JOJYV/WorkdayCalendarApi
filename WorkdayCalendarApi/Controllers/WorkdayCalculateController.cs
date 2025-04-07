using Microsoft.AspNetCore.Mvc;
using WorkayCalendarApi.Services.Interfaces;
using WorkdayCalendarApi.DTOs;

namespace WorkayCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WorkdayCalculateController : ControllerBase
    {
        private readonly IWorkdayService _workdayService;
        private readonly ILogger<WorkdayCalculateController> _logger;
        public WorkdayCalculateController(IWorkdayService workdayService, ILogger<WorkdayCalculateController> logger)
        {
            _workdayService = workdayService;
            _logger = logger;
        }

        // Calculate the resulting workday after adding or subtracting working days
        [HttpPost("calculate-date")]
        public async Task<IActionResult> CalculateWorkday([FromBody] WorkdayCalculationRequestDto request)
        {
            try
            {
                if (request.StartDate == default || request.WorkingDays == 0)
                {
                    return BadRequest("Invalid input parameters.");
                }
                DateTime resultDate = await _workdayService.CalculateTargetDate(
                    request.StartDate,
                    request.WorkingDays,
                    request.GetWorkStartTimeSpan(),
                    request.GetWorkEndTimeSpan()
                );
                return Ok(new { resultDate });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calculating the workday.");
                return StatusCode(500, "An unexpected error occurred while processing your request.");
            }
        }
    }
}
