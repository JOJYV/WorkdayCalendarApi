using System.ComponentModel.DataAnnotations;

namespace WorkdayCalendarApi.DTOs
{
    public class WorkdayCalculationRequestDto
    {
        public DateTime StartDate { get; set; }

        public double WorkingDays { get; set; }

        [Required(ErrorMessage = "WorkStartHour is required.")]
        [RegularExpression(@"^([0-9]{2}):([0-9]{2})$", ErrorMessage = "Invalid WorkStartHour format. Use HH:mm.")]
        public string WorkStartHour { get; set; }

        [Required(ErrorMessage = "WorkEndHour is required.")]
        [RegularExpression(@"^([0-9]{2}):([0-9]{2})$", ErrorMessage = "Invalid WorkEndHour format. Use HH:mm.")]
        public string WorkEndHour { get; set; }

        // Helper methods to convert strings to TimeSpan
        public TimeSpan GetWorkStartTimeSpan()
        {
            return TimeSpan.TryParse(WorkStartHour, out var startTime) ? startTime : TimeSpan.FromHours(8);// Default to 8 AM
        }

        public TimeSpan GetWorkEndTimeSpan()
        {
            return TimeSpan.TryParse(WorkEndHour, out var endTime) ? endTime : TimeSpan.FromHours(16);// Default to 4 PM
        }
    }
}