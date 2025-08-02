using FluentValidation;
using FluentValidation.Results;
using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Models;
using MeterReadingsApi.Repositories;
using MeterReadingsApi.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace MeterReadingsApi.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class MeterReadingUploadServiceTests
    {
        private static Mock<IFormFile> CreateFile()
        {
            MemoryStream stream = new MemoryStream(new byte[] { 1 });
            Mock<IFormFile> mock = new Mock<IFormFile>();
            mock.Setup(f => f.OpenReadStream()).Returns(stream);
            mock.Setup(f => f.Length).Returns(stream.Length);

            return mock;
        }

        private static ValidationResult Valid() => new ValidationResult();
        private static ValidationResult Invalid() => new ValidationResult(new[] { new ValidationFailure("MeterReadValue", "error") });

        [Fact]
        public async Task UploadAsync_Adds_valid_readings_and_returns_counts()
        {
            // Arrange
            MeterReadingCsvRecord[] records = new[]
            {
                new MeterReadingCsvRecord { AccountId = 1, MeterReadingDateTime = "01/01/2020 00:00", MeterReadValue = "12345" },
                new MeterReadingCsvRecord { AccountId = 2, MeterReadingDateTime = "01/01/2020 00:00", MeterReadValue = "54321" }
            };

            Mock<ICSVService> csvService = new Mock<ICSVService>();
            csvService.Setup(s => s.ReadMeterReadingsAsync(It.IsAny<Stream>())).ReturnsAsync(records);

            Mock<IMeterReadingsRepository> repository = new Mock<IMeterReadingsRepository>();
            Mock<IValidator<MeterReadingCsvRecord>> validator = new Mock<IValidator<MeterReadingCsvRecord>>();
            validator.Setup(v => v.Validate(It.IsAny<MeterReadingCsvRecord>())).Returns(Valid());

            MeterReadingUploadService service = new MeterReadingUploadService(csvService.Object, repository.Object, validator.Object);

            Mock<IFormFile> file = CreateFile();

            // Act
            MeterReadingUploadResult result = await service.UploadAsync(file.Object);

            // Assert
            repository.Verify(r => r.AddMeterReadingsAsync(It.Is<IEnumerable<MeterReading>>(l => l != null && l.Count() == 2)), Times.Once);
            Assert.Equal(2, result.Successful);
            Assert.Equal(0, result.Failed);
        }

        [Fact]
        public async Task UploadAsync_Handles_invalid_records()
        {
            // Arrange
            MeterReadingCsvRecord[] records = new[]
            {
                new MeterReadingCsvRecord { AccountId = 1, MeterReadingDateTime = "01/01/2020 00:00", MeterReadValue = "12345" },
                new MeterReadingCsvRecord { AccountId = 2, MeterReadingDateTime = "01/01/2020 00:00", MeterReadValue = "99999" }
            };

            Mock<ICSVService> csvService = new Mock<ICSVService>();
            csvService.Setup(s => s.ReadMeterReadingsAsync(It.IsAny<Stream>())).ReturnsAsync(records);

            Mock<IMeterReadingsRepository> repository = new Mock<IMeterReadingsRepository>();
            Mock<IValidator<MeterReadingCsvRecord>> validator = new Mock<IValidator<MeterReadingCsvRecord>>();
            validator.SetupSequence(v => v.Validate(It.IsAny<MeterReadingCsvRecord>()))
                .Returns(Valid())
                .Returns(Invalid());

            MeterReadingUploadService service = new MeterReadingUploadService(csvService.Object, repository.Object, validator.Object);

            Mock<IFormFile> file = CreateFile();

            // Act
            MeterReadingUploadResult result = await service.UploadAsync(file.Object);

            // Assert
            repository.Verify(r => r.AddMeterReadingsAsync(It.Is<IEnumerable<MeterReading>>(l => l.Count() == 1)), Times.Once);
            Assert.Equal(1, result.Successful);
            Assert.Equal(1, result.Failed);
        }
    }
}