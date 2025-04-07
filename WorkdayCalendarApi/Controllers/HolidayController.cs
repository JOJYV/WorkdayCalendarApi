using Microsoft.AspNetCore.Mvc;
using WorkdayCalendarApi.Entities;
using WorkdayCalendarApi.DTOs;
using AutoMapper;
using WorkayCalendarApi.Services.Interfaces;

namespace WorkdayCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class HolidayController : ControllerBase
    {
        private readonly IHolidayService _holidayService;
        private readonly ILogger<HolidayController> _logger;
        private readonly IMapper _mapper;

        public HolidayController(IHolidayService holidayService, ILogger<HolidayController> logger, IMapper mapper)
        {
            _holidayService = holidayService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> AddHoliday([FromBody] HolidayDto holidayDto)
        {
            if (!holidayDto.IsRecurring && !holidayDto.Year.HasValue)
            {
                return BadRequest("Non-recurring holidays must have a specified Year.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var holidayEntity = _mapper.Map<HolidayEntity>(holidayDto);

            bool holidayExists = await _holidayService.HolidayExists(holidayEntity);

            if (holidayExists)
            {
                return BadRequest("This holiday already exists.");
            }

            try
            {

                await _holidayService.AddHoliday(holidayEntity);
                var holidayResponseDto = _mapper.Map<HolidayDto>(holidayEntity);
                return CreatedAtAction(nameof(GetAllHolidays), new { id = holidayEntity.PublicId }, holidayResponseDto);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a holiday.");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("all")]
        [ProducesResponseType(typeof(PagedHolidayResponseDto<HolidayDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PagedHolidayResponseDto<HolidayDto>>> GetAllHolidays(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string search = "")
        {
            try
            {
                var (holidays, totalCount) = await _holidayService.GetAllHolidays(pageNumber, pageSize, search);

                if (holidays == null || holidays.Count == 0)
                {
                    _logger.LogInformation("No holidays found.");
                    return NoContent();
                }

                var holidayDtos = _mapper.Map<List<HolidayDto>>(holidays);

                var result = new PagedHolidayResponseDto<HolidayDto>
                {
                    Holidays = holidayDtos,
                    TotalCount = totalCount
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching holidays.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteHoliday(Guid id)
        {
            try
            {
                await _holidayService.DeleteHoliday(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the holiday.");
                return StatusCode(500, "Internal server error Make sure Id exists.");
            }
        }
    }
}
