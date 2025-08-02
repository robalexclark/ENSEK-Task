using FluentValidation.Results;
using MeterReadingsApi.Models;
using MeterReadingsApi.Validators;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MeterReadingsApi.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class MeterReadingUploadRequestValidatorTests
    {
        private static IFormFile CreateFile(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            MemoryStream stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "file", "test.csv");
        }

        [Fact]
        public void Validate_ReturnsError_When_BlankRowPresent()
        {
            // Arrange
            string csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                         "2344,16/05/2019 09:24,00123\n" +
                         "\n";
            MeterReadingUploadRequest request = new MeterReadingUploadRequest
            {
                File = CreateFile(csv)
            };
            MeterReadingUploadRequestValidator validator = new MeterReadingUploadRequestValidator();

            // Act
            ValidationResult result = validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("blank rows"));
        }

        [Fact]
        public void Validate_ReturnsError_When_HeaderRowIsBlank()
        {
            // Arrange
            string csv = "\n" +
                         "2344,16/05/2019 09:24,00123\n";
            MeterReadingUploadRequest request = new MeterReadingUploadRequest
            {
                File = CreateFile(csv)
            };
            MeterReadingUploadRequestValidator validator = new MeterReadingUploadRequestValidator();

            // Act
            ValidationResult result = validator.Validate(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("Invalid or missing headers"));
        }
    }
}