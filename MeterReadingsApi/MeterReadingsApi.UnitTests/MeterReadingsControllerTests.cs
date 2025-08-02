using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MeterReadingsApi.Controllers;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Models;
using MeterReadingsApi.Shared;
using MeterReadingsApi.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace MeterReadingsApi.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class MeterReadingsControllerTests
    {
        private static IFormFile CreateFile()
        {
            MemoryStream stream = new MemoryStream(new byte[] { 1 });
            return new FormFile(stream, 0, stream.Length, "file", "test.csv");
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_BadRequest_When_File_Invalid()
        {
            // Arrange
            Mock<IMeterReadingUploadService> service = new Mock<IMeterReadingUploadService>();
            Mock<IMeterReadingsRepository> repo = new Mock<IMeterReadingsRepository>();
            Mock<IValidator<int>> validator = new Mock<IValidator<int>>();
            Mock<IValidator<MeterReadingUploadRequest>> fileValidator = new Mock<IValidator<MeterReadingUploadRequest>>();
            fileValidator.Setup(v => v.Validate(It.IsAny<MeterReadingUploadRequest>()))
                .Returns(new ValidationResult(new[] { new ValidationFailure("File", "File contains blank rows") }));
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<MeterReading, MeterReadingDto>()).CreateMapper();
            MeterReadingsController controller = new MeterReadingsController(service.Object, repo.Object, validator.Object, fileValidator.Object, mapper);

            // Act
            ActionResult result = await controller.MeterReadingUploads(new MeterReadingUploadRequest());

            // Assert
            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            IEnumerable<string> errors = Assert.IsAssignableFrom<IEnumerable<string>>(badRequest.Value);
            Assert.Contains("File contains blank rows", errors);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_Created_When_All_Successful()
        {
            // Arrange
            Mock<IMeterReadingUploadService> service = new Mock<IMeterReadingUploadService>();
            Mock<IMeterReadingsRepository> repo = new Mock<IMeterReadingsRepository>();
            MeterReadingUploadResult uploadResult = new MeterReadingUploadResult(1, 0, Array.Empty<MeterReadingUploadFailure>());
            service.Setup(s => s.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);

            Mock<IValidator<int>> validator = new Mock<IValidator<int>>();
            Mock<IValidator<MeterReadingUploadRequest>> fileValidator = new Mock<IValidator<MeterReadingUploadRequest>>();
            fileValidator.Setup(v => v.Validate(It.IsAny<MeterReadingUploadRequest>())).Returns(new ValidationResult());
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<MeterReading, MeterReadingDto>()).CreateMapper();
            MeterReadingsController controller = new MeterReadingsController(service.Object, repo.Object, validator.Object, fileValidator.Object, mapper);
            IFormFile file = CreateFile();

            // Act
            ActionResult result = await controller.MeterReadingUploads(new MeterReadingUploadRequest { File = file });

            // Assert
            CreatedResult created = Assert.IsType<CreatedResult>(result);
            Assert.Equal(uploadResult, created.Value);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_MultiStatus_When_Some_Fail()
        {
            // Arrange
            Mock<IMeterReadingUploadService> service = new Mock<IMeterReadingUploadService>();
            Mock<IMeterReadingsRepository> repo = new Mock<IMeterReadingsRepository>();
            MeterReadingUploadResult uploadResult = new MeterReadingUploadResult(1, 1, new[] { new MeterReadingUploadFailure(2, null, "error") });
            service.Setup(s => s.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);

            Mock<IValidator<int>> validator = new Mock<IValidator<int>>();
            Mock<IValidator<MeterReadingUploadRequest>> fileValidator = new Mock<IValidator<MeterReadingUploadRequest>>();
            fileValidator.Setup(v => v.Validate(It.IsAny<MeterReadingUploadRequest>())).Returns(new ValidationResult());
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<MeterReading, MeterReadingDto>()).CreateMapper();
            MeterReadingsController controller = new MeterReadingsController(service.Object, repo.Object, validator.Object, fileValidator.Object, mapper);
            IFormFile file = CreateFile();

            // Act
            ActionResult result = await controller.MeterReadingUploads(new MeterReadingUploadRequest { File = file });

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status207MultiStatus, objectResult.StatusCode);
            Assert.Equal(uploadResult, objectResult.Value);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_UnprocessableEntity_When_All_Fail()
        {
            // Arrange
            Mock<IMeterReadingUploadService> service = new Mock<IMeterReadingUploadService>();
            Mock<IMeterReadingsRepository> repo = new Mock<IMeterReadingsRepository>();
            MeterReadingUploadResult uploadResult = new MeterReadingUploadResult(0, 1, new[] { new MeterReadingUploadFailure(2, null, "error") });
            service.Setup(s => s.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);

            Mock<IValidator<int>> validator = new Mock<IValidator<int>>();
            Mock<IValidator<MeterReadingUploadRequest>> fileValidator = new Mock<IValidator<MeterReadingUploadRequest>>();
            fileValidator.Setup(v => v.Validate(It.IsAny<MeterReadingUploadRequest>())).Returns(new ValidationResult());
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<MeterReading, MeterReadingDto>()).CreateMapper();
            MeterReadingsController controller = new MeterReadingsController(service.Object, repo.Object, validator.Object, fileValidator.Object, mapper);
            IFormFile file = CreateFile();

            // Act
            ActionResult result = await controller.MeterReadingUploads(new MeterReadingUploadRequest { File = file });

            // Assert
            UnprocessableEntityObjectResult unprocessable = Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal(uploadResult, unprocessable.Value);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public void GetByAccountId_Returns_Readings()
        {
            // Arrange
            Mock<IMeterReadingUploadService> service = new Mock<IMeterReadingUploadService>();
            Mock<IMeterReadingsRepository> repo = new Mock<IMeterReadingsRepository>();
            MeterReading reading = new MeterReading { AccountId = 1, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = 100 };
            repo.Setup(r => r.GetMeterReadingsByAccountId(1)).Returns(new[] { reading });
            Mock<IValidator<int>> validator = new Mock<IValidator<int>>();
            validator.Setup(v => v.Validate(It.IsAny<int>())).Returns(new ValidationResult());
            Mock<IValidator<MeterReadingUploadRequest>> fileValidator = new Mock<IValidator<MeterReadingUploadRequest>>();
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<MeterReading, MeterReadingDto>()).CreateMapper();
            MeterReadingsController controller = new MeterReadingsController(service.Object, repo.Object, validator.Object, fileValidator.Object, mapper);

            // Act
            ActionResult result = controller.GetByAccountId(1);

            // Assert
            OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
            IEnumerable<MeterReadingDto> readings = Assert.IsAssignableFrom<IEnumerable<MeterReadingDto>>(ok.Value);
            MeterReadingDto dto = Assert.Single(readings);
            Assert.Equal(reading.AccountId, dto.AccountId);
            Assert.Equal(reading.MeterReadingDateTime, dto.MeterReadingDateTime);
            Assert.Equal(reading.MeterReadValue, dto.MeterReadValue);
        }

        [Fact]
        public void GetByAccountId_Returns_NoContent_When_No_Readings()
        {
            // Arrange
            Mock<IMeterReadingUploadService> service = new Mock<IMeterReadingUploadService>();
            Mock<IMeterReadingsRepository> repo = new Mock<IMeterReadingsRepository>();
            repo.Setup(r => r.GetMeterReadingsByAccountId(1)).Returns(Array.Empty<MeterReading>());
            Mock<IValidator<int>> validator = new Mock<IValidator<int>>();
            validator.Setup(v => v.Validate(It.IsAny<int>())).Returns(new ValidationResult());
            Mock<IValidator<MeterReadingUploadRequest>> fileValidator = new Mock<IValidator<MeterReadingUploadRequest>>();
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<MeterReading, MeterReadingDto>()).CreateMapper();
            MeterReadingsController controller = new MeterReadingsController(service.Object, repo.Object, validator.Object, fileValidator.Object, mapper);

            // Act
            ActionResult result = controller.GetByAccountId(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void GetByAccountId_Returns_NotFound_When_Account_Does_Not_Exist()
        {
            // Arrange
            Mock<IMeterReadingUploadService> service = new Mock<IMeterReadingUploadService>();
            Mock<IMeterReadingsRepository> repo = new Mock<IMeterReadingsRepository>();
            Mock<IValidator<int>> validator = new Mock<IValidator<int>>();
            validator.Setup(v => v.Validate(It.IsAny<int>())).Returns(new ValidationResult(new[] { new ValidationFailure("AccountId", "error") }));
            Mock<IValidator<MeterReadingUploadRequest>> fileValidator = new Mock<IValidator<MeterReadingUploadRequest>>();
            IMapper mapper = new MapperConfiguration(cfg => cfg.CreateMap<MeterReading, MeterReadingDto>()).CreateMapper();
            MeterReadingsController controller = new MeterReadingsController(service.Object, repo.Object, validator.Object, fileValidator.Object, mapper);

            // Act
            ActionResult result = controller.GetByAccountId(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}