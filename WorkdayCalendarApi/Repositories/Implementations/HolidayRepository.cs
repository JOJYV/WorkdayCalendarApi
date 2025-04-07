using Microsoft.EntityFrameworkCore;
using WorkayCalendarApi.Repositories.Interfaces;
using WorkdayCalendarApi.Entities;

namespace WorkayCalendarApi.Repositories.Implementations
{
    public class HolidayRepository : IHolidayRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HolidayRepository> _logger;

        public HolidayRepository(ApplicationDbContext context, ILogger<HolidayRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<(List<HolidayEntity> Holidays, int TotalCount)> GetHolidays(int pageNumber, int pageSize, string searchQuery)
        {
            var query = _context.Holidays.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(h => h.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            var totalCount = await query.CountAsync();

            if (totalCount == 0)
            {
                return (new List<HolidayEntity>(), totalCount);
            }

            var holidays = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (holidays, totalCount);

        }


        public async Task AddHoliday(HolidayEntity holiday)
        {
            try
            {
                if (holiday == null)
                {
                    throw new ArgumentNullException(nameof(holiday), "Holiday cannot be null.");
                }
                holiday.PublicId = Guid.NewGuid();
                await _context.Holidays.AddAsync(holiday);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Holiday '{holiday.Name}' added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a holiday.");
                throw new ApplicationException("An error occurred while adding the holiday.");
            }
        }

        public async Task DeleteHoliday(Guid id)
        {
            try
            {
                var holiday = await _context.Holidays.FirstOrDefaultAsync(h => h.PublicId == id);
                if (holiday == null)
                {
                    _logger.LogWarning($"Holiday with ID {id} not found for deletion.");
                    throw new ApplicationException("invalid Id");
                    
                }
                _context.Holidays.Remove(holiday);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Holiday with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting a holiday.");
                throw new ApplicationException("An error occurred while deleting the holiday.");
            }
        }

        public async Task<bool> HolidayExists(HolidayEntity holidayEntity)
        {
            if (holidayEntity.IsRecurring)
            {
                return await _context.Holidays
                    .AnyAsync(h => h.Month == holidayEntity.Month && h.Day == holidayEntity.Day && h.IsRecurring 
                                   );
            }
            else
            {
                return await _context.Holidays
                    .AnyAsync(h => h.Month == holidayEntity.Month && h.Day == holidayEntity.Day && h.Year == holidayEntity.Year &&
                                   !h.IsRecurring);
            }
        }


    }
}
