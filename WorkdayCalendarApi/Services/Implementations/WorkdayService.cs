using WorkayCalendarApi.Repositories.Interfaces;
using WorkayCalendarApi.Services.Interfaces;
using WorkdayCalendarApi.Entities;

namespace WorkayCalendarApi.Services.Implementations
{
    public class WorkdayService : IWorkdayService
    {
        private readonly IHolidayRepository _holidayRepository;

        public WorkdayService(IHolidayRepository holidayRepository)
        {
            _holidayRepository = holidayRepository;
        }

        public async Task<DateTime> CalculateTargetDate(DateTime startDate, double workingDays, TimeSpan workStartTime, TimeSpan workEndTime)
        {
            DateTime workStartDate = RoundToMinute(startDate);
            double remainingDays = workingDays;
            double workdayHours = (workEndTime - workStartTime).TotalHours;
            var holidays = await GetAllHolidays();// To Check ,If Caching can be used depending on the Size of Holidays      

            while (Math.Abs(remainingDays) > 1e-6) // avoid floating point precision issues
            {
                if (IsWorkingDay(workStartDate, holidays))
                {
                    if (workStartDate.TimeOfDay < workStartTime) //If time is before working hours, start at workStartTime.
                        workStartDate = workStartDate.Date + workStartTime;
                    else if (workStartDate.TimeOfDay > workEndTime)//
                        workStartDate = remainingDays > 0//If time is after working hours, move to next or previous working day, based on the direction(positive or negative remainingDays).
                            ? workStartDate.Date.AddDays(1) + workStartTime
                            : workStartDate.Date.AddDays(-1) + workEndTime;
                    double remainingHours = remainingDays * workdayHours;

                    if (remainingHours > 0) //Moving Forward in Time(Remaining Days > 0)
                    {
                        double hoursToEndOfDay = (workEndTime - workStartDate.TimeOfDay).TotalHours;// Calculates how many working hours are left today.
                        if (remainingHours <= hoursToEndOfDay) //Enough time left today → Just add and finish.
                        {
                            workStartDate = RoundToMinute(workStartDate.AddHours(remainingHours));
                            remainingDays = 0;
                        }
                        else//Not enough → use today's hours, then move to next working day and deduct fraction used.
                        {
                            workStartDate = RoundToMinute(workStartDate.Date.AddDays(1) + workStartTime);
                            remainingDays -= hoursToEndOfDay / workdayHours;
                        }
                    }
                    else
                    {
                        double hoursFromStartOfDay = (workStartDate.TimeOfDay - workStartTime).TotalHours;
                        if (Math.Abs(remainingHours) <= hoursFromStartOfDay)
                        {
                            workStartDate = RoundToMinute(workStartDate.AddHours(remainingHours));
                            remainingDays = 0;
                        }
                        else
                        {
                            workStartDate = RoundToMinute(workStartDate.Date.AddDays(-1) + workEndTime);
                            remainingDays += hoursFromStartOfDay / workdayHours;
                        }
                    }
                }
                else
                {
                    workStartDate = remainingDays > 0
                        ? workStartDate.Date.AddDays(1) + workStartTime
                        : workStartDate.Date.AddDays(-1) + workEndTime;

                    workStartDate = RoundToMinute(workStartDate);
                }
            }

            return workStartDate;
        }



        private DateTime RoundToMinute(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        }


        // To do// while fetching holidays,Instead of fetching all holidays, we can fetch only the required holidays based on the date range+ all recurring holidays
        //To do// Keep in mind that this is a simple implementation and may not be optimal for large datasets Or do caching for limited records with some expiry
        private async Task<List<HolidayEntity>> GetAllHolidays()
        {
            var allHolidays = new List<HolidayEntity>();
            int pageNumber = 1;
            int pageSize = 1000;
            int totalCount;


            do
            {
                var holidaysPage = await _holidayRepository.GetHolidays(pageNumber, pageSize, string.Empty);

                allHolidays.AddRange(holidaysPage.Holidays);

                totalCount = holidaysPage.TotalCount;
                pageNumber++;

            } while (allHolidays.Count < totalCount); // Continue until all holidays are fetched

            return allHolidays;
        }

        private bool IsWorkingDay(DateTime date, List<HolidayEntity> holidays)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }
            foreach (var holiday in holidays)
            {
                if (holiday.IsActive &&
                    ((holiday.IsRecurring && holiday.Month == date.Month && holiday.Day == date.Day) ||
                    (!holiday.IsRecurring && holiday.Year == date.Year && holiday.Month == date.Month && holiday.Day == date.Day)))
                {
                    return false;
                }
            }

            return true;
        }


    }
}
