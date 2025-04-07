using Microsoft.EntityFrameworkCore;
using Moq;
using WorkayCalendarApi.Repositories.Implementations;
using WorkdayCalendarApi.Entities;
using Xunit;

public class HolidayRepositoryTests
{
    private HolidayRepository CreateRepository(ApplicationDbContext context)
    {
        var mockLogger = new Mock<ILogger<HolidayRepository>>();
        return new HolidayRepository(context, mockLogger.Object);
    }

    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "HolidayDatabase")
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task GetHolidays_ShouldReturnCorrectHolidays()
    {
        var context = CreateDbContext();
        context.Holidays.AddRange(
            new HolidayEntity { Id = 1, Name = "May17", Day = 17, Month = 5, Year = null, IsRecurring = true, IsActive = true },
            new HolidayEntity { Id = 2, Name = "May27", Day = 27, Month = 5, Year = 2004, IsRecurring = false, IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = CreateRepository(context);

        var (holidays, totalCount) = await repository.GetHolidays(1, 10, "May");

        Assert.Equal(2, totalCount);
        Assert.Equal(2, holidays.Count);
        Assert.Contains(holidays, h => h.Name == "May17");
        Assert.Contains(holidays, h => h.Name == "May27");
    }

    [Fact]
    public async Task AddHoliday_ShouldAddHolidaySuccessfully()
    {
        var context = CreateDbContext();
        var repository = CreateRepository(context);
        var holiday = new HolidayEntity { Id = 3, Name = "NewHoliday", Day = 1, Month = 1, Year = 2025, IsRecurring = false, IsActive = true };

        await repository.AddHoliday(holiday);

        var addedHoliday = await context.Holidays.FindAsync(3);
        Assert.NotNull(addedHoliday);
        Assert.Equal("NewHoliday", addedHoliday.Name);
    }

    [Fact]
    public async Task DeleteHoliday_ShouldDeleteHolidaySuccessfully()
    {
        var context = CreateDbContext();
        var publicId= Guid.NewGuid();       
        context.Holidays.Add(new HolidayEntity { Id = 5, Name = "HolidayToDelete", PublicId= publicId, Day = 1, Month = 1, Year = 2025, IsRecurring = false, IsActive = true });
        await context.SaveChangesAsync();

        var repository = CreateRepository(context);

        await repository.DeleteHoliday(publicId);

        var deletedHoliday = await context.Holidays.FindAsync(5);
        Assert.Null(deletedHoliday);
    }

    [Fact]
    public async Task HolidayExists_ShouldReturnTrueForExistingHoliday()
    {
        var context = CreateDbContext();
        context.Holidays.Add(new HolidayEntity { Id = 6, Name = "ExistingHoliday", Day = 1, Month = 1, Year = 2025, IsRecurring = false, IsActive = true });
        await context.SaveChangesAsync();

        var repository = CreateRepository(context);
        var holiday = new HolidayEntity { Day = 1, Month = 1, Year = 2025, IsRecurring = false };

        var exists = await repository.HolidayExists(holiday);

        Assert.True(exists);
    }
}
