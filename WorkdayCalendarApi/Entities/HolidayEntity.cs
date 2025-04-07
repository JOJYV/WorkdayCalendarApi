using System.ComponentModel.DataAnnotations;

namespace WorkdayCalendarApi.Entities
{
    public class HolidayEntity
    {
        public int Id { get; set; }
        [StringLength(255)]// Primary Key
        public Guid PublicId { get; set; }
        public string Name { get; set; }      // Name of the holiday (e.g., "New Year's Day")
        public int Month { get; set; }        // Month of the holiday (1 = January, 12 = December)
        public int Day { get; set; }          // Day of the holiday (e.g., 1 for New Year's Day)
        public int? Year { get; set; }        // Year for non-recurring holidays (nullable)
        public bool IsRecurring { get; set; } // True if the holiday occurs every year
        public bool IsActive { get; set; }    // Flag to mark if the holiday is active
    }
}
