using MeterReadingsApi.Controllers;
using MeterReadingsApi.DataModel;
using MeterReadingsApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
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
            Mock<IMeterReadingsRepository> repo = new Mock<IMeterReadingsRepository>();
            Account account = new Account { AccountId = 1, FirstName = "A", LastName = "B" };
            repo.Setup(r => r.GetAccounts()).Returns(new[] { account });
            AccountsController controller = new AccountsController(repo.Object);

            // Act
            ActionResult result = controller.Get();

            // Assert
            OkObjectResult ok = Assert.IsType<OkObjectResult>(result);
            IEnumerable<Account> accounts = Assert.IsAssignableFrom<IEnumerable<Account>>(ok.Value);
            Assert.Single(accounts);
            Assert.Equal(account, accounts.First());
        }

        [Fact]
        public void Get_Returns_NoContent_When_No_Accounts()
        {
            // Arrange
            Mock<IMeterReadingsRepository> repo = new Mock<IMeterReadingsRepository>();
            repo.Setup(r => r.GetAccounts()).Returns(Array.Empty<Account>());
            AccountsController controller = new AccountsController(repo.Object);

            // Act
            ActionResult result = controller.Get();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
