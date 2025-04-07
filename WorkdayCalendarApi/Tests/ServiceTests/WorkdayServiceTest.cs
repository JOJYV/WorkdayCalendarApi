using Moq;
using WorkayCalendarApi.Repositories.Interfaces;
using WorkayCalendarApi.Services.Implementations;
using WorkdayCalendarApi.Entities;
using Xunit;
namespace WorkdayCalendarApi.Tests.ServiceTests
{
    public class WorkdayServiceTests
    {
        private WorkdayService CreateService()
        {
            var mockRepo = new Mock<IHolidayRepository>();

            mockRepo.Setup(r => r.GetHolidays(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((
                    new List<HolidayEntity>
                    {
                    // Recurring holiday - May 17
                    new HolidayEntity
                    {
                        Id = 1,
                        Name = "May17",
                        Day = 17,
                        Month = 5,
                        Year = null,
                        IsRecurring = true,
                        IsActive = true
                    },
                    // One-time holiday - May 27, 2004
                    new HolidayEntity
                    {
                        Id = 2,
                        Name = "May27",
                        Day = 27,
                        Month = 5,
                        Year = 2004,
                        IsRecurring = false,
                        IsActive = true
                    }
                    }, 2));

            return new WorkdayService(mockRepo.Object);
        }

        [Fact]
        public async Task AddQuarterDay_ShouldReturnCorrectTime()
        {
            var service = CreateService();
            var start = new DateTime(2004, 5, 24, 15, 7, 0);
            var result = await service.CalculateTargetDate(start, 0.25, TimeSpan.FromHours(8), TimeSpan.FromHours(16));

            Assert.Equal(new DateTime(2004, 5, 25, 9, 7, 0), result);
        }

        [Fact]
        public async Task AddHalfDayFromEarlyMorning_ShouldReturnSameDayNoon()
        {
            var service = CreateService();
            var start = new DateTime(2004, 5, 24, 4, 0, 0);
            var result = await service.CalculateTargetDate(start, 0.5, TimeSpan.FromHours(8), TimeSpan.FromHours(16));

            Assert.Equal(new DateTime(2004, 5, 24, 12, 0, 0), result);
        }

        [Fact]
        public async Task SubtractFiveAndHalfDays_ShouldSkipWeekendsAndHolidays()
        {
            var service = CreateService();
            var start = new DateTime(2004, 5, 24, 18, 5, 0);
            var result = await service.CalculateTargetDate(start, -5.5, TimeSpan.FromHours(8), TimeSpan.FromHours(16));

            Assert.Equal(new DateTime(2004, 5, 14, 12, 0, 0), result);
        }

        [Fact]
        public async Task AddLargeWorkingDays_ShouldReturnExpectedDate()
        {
            var service = CreateService();
            var start = new DateTime(2004, 5, 24, 19, 3, 0);
            var result = await service.CalculateTargetDate(start, 44.723656, TimeSpan.FromHours(8), TimeSpan.FromHours(16));

            Assert.Equal(new DateTime(2004, 7, 27, 13, 47, 0), result);
        }

        [Fact]
        public async Task SubtractLargeWorkingDays_ShouldReturnExpectedDate()
        {
            var service = CreateService();
            var start = new DateTime(2004, 5, 24, 18, 3, 0);
            var result = await service.CalculateTargetDate(start, -6.7470217, TimeSpan.FromHours(8), TimeSpan.FromHours(16));

            Assert.Equal(new DateTime(2004, 5, 13, 10, 1, 0), result);
        }

        [Fact]
        public async Task AddTwelvePointSevenDays_ShouldReturnExpectedDate()
        {
            var service = CreateService();
            var start = new DateTime(2004, 5, 24, 8, 3, 0);
            var result = await service.CalculateTargetDate(start, 12.782709, TimeSpan.FromHours(8), TimeSpan.FromHours(16));

            Assert.Equal(new DateTime(2004, 6, 10, 14, 18, 0), result);
        }

        [Fact]
        public async Task AddEightPointTwoDays_ShouldReturnExpectedDate()
        {
            var service = CreateService();
            var start = new DateTime(2004, 5, 24, 7, 3, 0);
            var result = await service.CalculateTargetDate(start, 8.276628, TimeSpan.FromHours(8), TimeSpan.FromHours(16));

            Assert.Equal(new DateTime(2004, 6, 4, 10, 12, 0), result);
        }
    }
}