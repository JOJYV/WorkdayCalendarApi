using WorkayCalendarApi.Services.Interfaces;
using WorkayCalendarApi.Repositories.Interfaces;
using WorkdayCalendarApi.Entities;

namespace WorkayCalendarApi.Services.Implementations
{


    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository _holidayRepository;

        public HolidayService(IHolidayRepository holidayRepository)
        {
            _holidayRepository = holidayRepository;
        }

        public async Task<(List<HolidayEntity> Holidays, int TotalCount)> GetAllHolidays(int pageNumber, int pageSize, string searchQuery)
        {
            return await _holidayRepository.GetHolidays(pageNumber, pageSize, searchQuery);
        }

        public async Task AddHoliday(HolidayEntity Holiday)
        {
            await _holidayRepository.AddHoliday(Holiday);
        }


        public async Task DeleteHoliday(Guid id)
        {
            await _holidayRepository.DeleteHoliday(id);
        }

        public async Task<bool> HolidayExists(HolidayEntity holidayEntity)
        {
            return await _holidayRepository.HolidayExists(holidayEntity);
        }
    }

}
