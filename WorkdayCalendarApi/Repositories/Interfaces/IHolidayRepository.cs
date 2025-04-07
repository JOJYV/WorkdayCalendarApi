using WorkdayCalendarApi.Entities;

namespace WorkayCalendarApi.Repositories.Interfaces
{
    public interface IHolidayRepository
    {

        Task AddHoliday(HolidayEntity Holiday);
        Task DeleteHoliday(Guid id);
        Task<(List<HolidayEntity> Holidays, int TotalCount)> GetHolidays(int pageNumber, int pageSize, string searchQuery);
        Task<bool> HolidayExists(HolidayEntity holidayEntity);
    }
}
