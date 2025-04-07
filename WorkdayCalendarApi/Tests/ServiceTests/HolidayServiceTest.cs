using Moq;
using WorkayCalendarApi.Repositories.Interfaces;
using WorkayCalendarApi.Services.Implementations;
using WorkdayCalendarApi.Entities;
using Xunit;

namespace WorkdayCalendarApi.Tests
{
    public class HolidayServiceTests
    {
        private (HolidayService service, Mock<IHolidayRepository> mockRepo) CreateService()
        {
            var mockRepo = new Mock<IHolidayRepository>();


            mockRepo.Setup(r => r.GetHolidays(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((new List<HolidayEntity>
                {
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


            mockRepo.Setup(r => r.AddHoliday(It.IsAny<HolidayEntity>()))
                .Returns(Task.CompletedTask);


            mockRepo.Setup(r => r.DeleteHoliday(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);


            mockRepo.Setup(r => r.HolidayExists(It.IsAny<HolidayEntity>()))
                .ReturnsAsync(true);

            var service = new HolidayService(mockRepo.Object);
            return (service, mockRepo);
        }

        [Fact]
        public async Task GetAllHolidays_ShouldReturnListOfHolidays()
        {

            var (service, _) = CreateService();


            var result = await service.GetAllHolidays(1, 10, string.Empty);


            Assert.NotNull(result.Holidays);
            Assert.Equal(2, result.Holidays.Count);
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task AddHoliday_ShouldCallRepositoryMethod()
        {

            var (service, mockRepo) = CreateService();
            var holiday = new HolidayEntity
            {
                Id = 3,
                Name = "NewHoliday",
                Day = 15,
                Month = 8,
                Year = 2025,
                IsRecurring = false,
                IsActive = true
            };


            await service.AddHoliday(holiday);


            mockRepo.Verify(r => r.AddHoliday(It.Is<HolidayEntity>(h => h.Name == "NewHoliday")), Times.Once);
        }


        [Fact]
        public async Task DeleteHoliday_ShouldCallRepositoryMethod()
        {

            var (service, mockRepo) = CreateService();
            var holidayId = Guid.NewGuid();


            await service.DeleteHoliday(holidayId);


            mockRepo.Verify(r => r.DeleteHoliday(It.Is<Guid>(id => id == holidayId)), Times.Once);
        }

        [Fact]
        public async Task HolidayExists_ShouldReturnTrueWhenHolidayExists()
        {

            var (service, _) = CreateService();
            var holiday = new HolidayEntity
            {
                Id = 1,
                Name = "May17",
                Day = 17,
                Month = 5,
                Year = null,
                IsRecurring = true,
                IsActive = true
            };


            var result = await service.HolidayExists(holiday);


            Assert.True(result);
        }
    }
}
