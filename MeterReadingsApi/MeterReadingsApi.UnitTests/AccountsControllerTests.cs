using AutoMapper;
using MeterReadingsApi.Controllers;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Models;
using MeterReadingsApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace MeterReadingsApi.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class AccountsControllerTests
    {
        [Fact]
        public void Get_Returns_Accounts()
        {
            // Arrange
            Mock<IMeterReadingsRepository> repo = new();
            Account account = new() { AccountId = 1, FirstName = "A", LastName = "B" };
            repo.Setup(r => r.GetAccounts()).Returns(new[] { account });

            Mock<IMapper> mapper = new();
            AccountDto dto = new() { AccountId = 1, FirstName = "A", LastName = "B" };
            mapper.Setup(m => m.Map<IEnumerable<AccountDto>>(It.IsAny<IEnumerable<Account>>()))
                  .Returns(new[] { dto });

            AccountsController controller = new(repo.Object, mapper.Object);

            // Act
            ActionResult result = controller.Get();

            // Assert
            OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
            IEnumerable<AccountDto> accounts = Assert.IsAssignableFrom<IEnumerable<AccountDto>>(ok.Value);
            AccountDto returned = Assert.Single(accounts);
            Assert.Equal(dto.AccountId, returned.AccountId);
            Assert.Equal(dto.FirstName, returned.FirstName);
            Assert.Equal(dto.LastName, returned.LastName);
        }

        [Fact]
        public void Get_Returns_NoContent_When_No_Accounts()
        {
            // Arrange
            Mock<IMeterReadingsRepository> repo = new();
            repo.Setup(r => r.GetAccounts()).Returns(Array.Empty<Account>());
            Mock<IMapper> mapper = new();
            AccountsController controller = new(repo.Object, mapper.Object);

            // Act
            ActionResult result = controller.Get();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}