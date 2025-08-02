using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using FluentValidation.Results;
using MeterReadingsApi.CsvMappers;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Repositories;
using MeterReadingsApi.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace MeterReadingsApi.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class MeterReadingUploadServiceTests
    {
        private static Mock<IFormFile> CreateFile()
        {
            var stream = new MemoryStream(new byte[] {1});
            var mock = new Mock<IFormFile>();
            mock.Setup(f => f.OpenReadStream()).Returns(stream);
            mock.Setup(f => f.Length).Returns(stream.Length);
            return mock;
        }

        private static ValidationResult Valid() => new ValidationResult();
        private static ValidationResult Invalid() => new ValidationResult(new[] { new ValidationFailure("MeterReadValue", "error") });

        [Fact]
        public async Task UploadAsync_Adds_valid_readings_and_returns_counts()
        {
            // arrange
            var records = new[]
            {
                new MeterReadingCsvRecord { AccountId = 1, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = "12345" },
                new MeterReadingCsvRecord { AccountId = 2, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = "54321" }
            };

            var csvService = new Mock<ICSVService>();
            csvService.Setup(s => s.ReadMeterReadingsAsync(It.IsAny<Stream>())).ReturnsAsync(records);

            var repository = new Mock<IMeterReadingsRepository>();
            var validator = new Mock<IValidator<MeterReadingCsvRecord>>();
            validator.Setup(v => v.Validate(It.IsAny<MeterReadingCsvRecord>())).Returns(Valid());

            var service = new MeterReadingUploadService(csvService.Object, repository.Object, validator.Object);

            var file = CreateFile();

            // act
            var result = await service.UploadAsync(file.Object);

            // assert
            repository.Verify(r => r.AddMeterReadingsAsync(It.Is<IEnumerable<MeterReading>>(l => l != null && l.Count() == 2)), Times.Once);
            Assert.Equal(2, result.Successful);
            Assert.Equal(0, result.Failed);
        }

        [Fact]
        public async Task UploadAsync_Handles_invalid_records()
        {
            // arrange
            var records = new[]
            {
                new MeterReadingCsvRecord { AccountId = 1, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = "12345" },
                new MeterReadingCsvRecord { AccountId = 2, MeterReadingDateTime = DateTime.UtcNow, MeterReadValue = "99999" }
            };

            var csvService = new Mock<ICSVService>();
            csvService.Setup(s => s.ReadMeterReadingsAsync(It.IsAny<Stream>())).ReturnsAsync(records);

            var repository = new Mock<IMeterReadingsRepository>();
            var validator = new Mock<IValidator<MeterReadingCsvRecord>>();
            validator.SetupSequence(v => v.Validate(It.IsAny<MeterReadingCsvRecord>()))
                .Returns(Valid())
                .Returns(Invalid());

            var service = new MeterReadingUploadService(csvService.Object, repository.Object, validator.Object);

            var file = CreateFile();

            // act
            var result = await service.UploadAsync(file.Object);

            // assert
            repository.Verify(r => r.AddMeterReadingsAsync(It.Is<IEnumerable<MeterReading>>(l => l.Count() == 1)), Times.Once);
            Assert.Equal(1, result.Successful);
            Assert.Equal(1, result.Failed);
        }
    }
}
