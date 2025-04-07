
using Microsoft.AspNetCore.Mvc;
using Moq;
using WorkayCalendarApi.Controllers;
using WorkayCalendarApi.Services.Interfaces;
using WorkdayCalendarApi.DTOs;
using Xunit;
namespace WorkdayCalendarApi.Tests.ControllerTests
{
    public class WorkdayCalculateControllerTests
    {
        private WorkdayCalculateController CreateController(IWorkdayService workdayService)
        {
            var mockLogger = new Mock<ILogger<WorkdayCalculateController>>();
            return new WorkdayCalculateController(workdayService, mockLogger.Object);
        }

        [Fact]
        public async Task CalculateWorkday_ShouldReturnOkObjectResult()
        {
            var mockService = new Mock<IWorkdayService>();
            var controller = CreateController(mockService.Object);

            var request = new WorkdayCalculationRequestDto
            {
                StartDate = new DateTime(2025, 4, 6, 9, 0, 0),
                WorkingDays = 5,
                WorkStartHour = "08:00",
                WorkEndHour = "16:00"
            };

            var expectedDate = new DateTime(2025, 4, 13, 9, 0, 0);
            mockService.Setup(s => s.CalculateTargetDate(It.IsAny<DateTime>(), It.IsAny<double>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(expectedDate);

            var result = await controller.CalculateWorkday(request);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            dynamic resultValue = actionResult.Value;
            Assert.Equal(expectedDate, resultValue.resultDate);
        }



        [Fact]
        public async Task CalculateWorkday_ShouldReturnBadRequest_WhenInvalidInput()
        {
            var mockService = new Mock<IWorkdayService>();
            var controller = CreateController(mockService.Object);

            var request = new WorkdayCalculationRequestDto
            {
                StartDate = default,
                WorkingDays = 0,
                WorkStartHour = "08:00",
                WorkEndHour = "16:00"
            };

            var result = await controller.CalculateWorkday(request);

            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid input parameters.", actionResult.Value);
        }

        [Fact]
        public async Task CalculateWorkday_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            var mockService = new Mock<IWorkdayService>();
            var controller = CreateController(mockService.Object);

            var request = new WorkdayCalculationRequestDto
            {
                StartDate = new DateTime(2025, 4, 6, 9, 0, 0),
                WorkingDays = 5,
                WorkStartHour = "08:00",
                WorkEndHour = "16:00"
            };

            mockService.Setup(s => s.CalculateTargetDate(It.IsAny<DateTime>(), It.IsAny<double>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .ThrowsAsync(new Exception("Test exception"));

            var result = await controller.CalculateWorkday(request);

            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, actionResult.StatusCode);
            Assert.Equal("An unexpected error occurred while processing your request.", actionResult.Value);
        }
    }
}