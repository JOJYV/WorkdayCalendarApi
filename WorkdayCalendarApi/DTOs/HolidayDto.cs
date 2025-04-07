using System.ComponentModel.DataAnnotations;

namespace WorkdayCalendarApi.DTOs
{
    public class HolidayDto
    {
        
        public Guid Id { get; internal set; }

        [Required(ErrorMessage = "Holiday name is required.")]
        [StringLength(255, ErrorMessage = "Holiday name can't exceed 255 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Month is required.")]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int Month { get; set; }

        [Required(ErrorMessage = "Day is required.")]
        [Range(1, 31, ErrorMessage = "Day must be between 1 and 31.")]
        public int Day { get; set; }

        [Range(1, 9999, ErrorMessage = "Year must be a valid number.")]
        public int? Year { get; set; }

        [Required(ErrorMessage = "IsRecurring is required.")]
        public bool IsRecurring { get; set; }

        [Required(ErrorMessage = "IsActive is required.")]
        public bool IsActive { get; set; }

    }
}
