using Microsoft.EntityFrameworkCore;
using WorkdayCalendarApi.Entities;

namespace WorkayCalendarApi.Services.Interfaces
{
    public interface IWorkdayService
    {

        Task<DateTime> CalculateTargetDate(DateTime startDate, double workingDays, TimeSpan workStartTime, TimeSpan workEndTime);

    }
}
