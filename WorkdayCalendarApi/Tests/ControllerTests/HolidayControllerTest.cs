using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WorkdayCalendarApi.Controllers;
using WorkdayCalendarApi.DTOs;
using WorkdayCalendarApi.Entities;
using WorkayCalendarApi.Services.Interfaces;
using Xunit;

namespace WorkdayCalendarApi.Tests.Controllers
{
    public class HolidayControllerTests
    {
        private readonly Mock<IHolidayService> _mockHolidayService;
        private readonly Mock<ILogger<HolidayController>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly HolidayController _controller;

        public HolidayControllerTests()
        {
            _mockHolidayService = new Mock<IHolidayService>();
            _mockLogger = new Mock<ILogger<HolidayController>>();
            _mockMapper = new Mock<IMapper>();
            _controller = new HolidayController(_mockHolidayService.Object, _mockLogger.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task AddHoliday_ReturnsBadRequest_WhenNonRecurringHolidayWithoutYear()
        {
            // Arrange
            var holidayDto = new HolidayDto { IsRecurring = false, Year = null };

            // Act
            var result = await _controller.AddHoliday(holidayDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Non-recurring holidays must have a specified Year.", badRequestResult.Value);
        }

        [Fact]
        public async Task AddHoliday_ReturnsBadRequest_WhenHolidayExists()
        {
            // Arrange
            var holidayDto = new HolidayDto { IsRecurring = true };
            var holidayEntity = new HolidayEntity();
            _mockMapper.Setup(m => m.Map<HolidayEntity>(holidayDto)).Returns(holidayEntity);
            _mockHolidayService.Setup(s => s.HolidayExists(holidayEntity)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddHoliday(holidayDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("This holiday already exists.", badRequestResult.Value);
        }

        [Fact]
        public async Task AddHoliday_ReturnsCreatedAtAction_WhenHolidayIsAdded()
        {
            // Arrange
            var holidayDto = new HolidayDto { IsRecurring = true };
            var holidayEntity = new HolidayEntity { PublicId = Guid.NewGuid() };
            _mockMapper.Setup(m => m.Map<HolidayEntity>(holidayDto)).Returns(holidayEntity);
            _mockHolidayService.Setup(s => s.HolidayExists(holidayEntity)).ReturnsAsync(false);
            _mockHolidayService.Setup(s => s.AddHoliday(holidayEntity)).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<HolidayDto>(holidayEntity)).Returns(holidayDto);

            // Act
            var result = await _controller.AddHoliday(holidayDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(HolidayController.GetAllHolidays), createdAtActionResult.ActionName);
            Assert.Equal(holidayEntity.PublicId, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(holidayDto, createdAtActionResult.Value);
        }

        [Fact]
        public async Task AddHoliday_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var holidayDto = new HolidayDto { IsRecurring = true };
            var holidayEntity = new HolidayEntity();
            _mockMapper.Setup(m => m.Map<HolidayEntity>(holidayDto)).Returns(holidayEntity);
            _mockHolidayService.Setup(s => s.HolidayExists(holidayEntity)).ReturnsAsync(false);
            _mockHolidayService.Setup(s => s.AddHoliday(holidayEntity)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.AddHoliday(holidayDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);
        }
    }
}
