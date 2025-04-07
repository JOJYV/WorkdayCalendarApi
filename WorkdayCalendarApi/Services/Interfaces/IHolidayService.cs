using WorkdayCalendarApi.Entities;

namespace WorkayCalendarApi.Services.Interfaces
{
    public interface IHolidayService
    {

        Task<(List<HolidayEntity> Holidays, int TotalCount)> GetAllHolidays(int pageNumber, int pageSize, string searchQuery);

        Task AddHoliday(HolidayEntity Holiday);

        Task DeleteHoliday(Guid id);
        Task<bool> HolidayExists(HolidayEntity holidayEntity);
    }
}
