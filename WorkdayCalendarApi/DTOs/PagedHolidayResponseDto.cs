namespace WorkdayCalendarApi.DTOs
{
    public class PagedHolidayResponseDto<T>
    {
        public List<T> Holidays { get; set; }
        public int TotalCount { get; set; }
    }
}
