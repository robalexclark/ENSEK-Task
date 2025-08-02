using FluentValidation.Results;
using MeterReadingsApi.Repositories;
using MeterReadingsApi.Validators;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace MeterReadingsApi.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class AccountIdValidatorTests
    {
        [Fact]
        public void Validate_ReturnsValid_WhenAccountExists()
        {
            // Arrange
            Mock<IMeterReadingsRepository> repo = new Mock<IMeterReadingsRepository>();
            repo.Setup(r => r.AccountExists(1)).Returns(true);
            AccountIdValidator validator = new AccountIdValidator(repo.Object);

            // Act
            ValidationResult result = validator.Validate(1);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_ReturnsInvalid_WhenAccountDoesNotExist()
        {
            // Arrange
            Mock<IMeterReadingsRepository> repo = new Mock<IMeterReadingsRepository>();
            repo.Setup(r => r.AccountExists(1)).Returns(false);
            AccountIdValidator validator = new AccountIdValidator(repo.Object);

            // Act
            ValidationResult result = validator.Validate(1);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Account does not exist");
        }
    }
}