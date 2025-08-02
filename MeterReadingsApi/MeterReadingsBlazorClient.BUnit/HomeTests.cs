using Bunit;
using MeterReadingsApi.Shared;
using MeterReadingsBlazorClient.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;

namespace MeterReadingsBlazorClient.BUnit;

public partial class HomeTests : BlazoriseTestBase
{
    [Fact]
    public void Home_LoadsAccountsOnInit()
    {
        // Arrange
        TestHttpMessageHandler handler = new(req =>
        {
            if (req.RequestUri?.AbsolutePath == "/accounts")
            {
                AccountDto[] accounts = new[] { new AccountDto { AccountId = 1, FirstName = "Jane", LastName = "Doe" } };
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(accounts)
                };
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });
        HttpClient client = new(handler) { BaseAddress = new Uri("http://localhost") };
        Services.AddSingleton(client);

        // Act
        IRenderedComponent<Home> cut = RenderComponent<Home>();

        // Assert
        FieldInfo? accountsField = typeof(Home).GetField("accounts", BindingFlags.NonPublic | BindingFlags.Instance);
        IList<AccountDto>? accounts = accountsField?.GetValue(cut.Instance) as IList<AccountDto>;
        Assert.NotNull(accounts);
        Assert.Single(accounts!);
        Assert.Equal("Jane", accounts![0].FirstName);
    }

    [Fact]
    public async Task Home_LoadsMeterReadingsWhenAccountSelected()
    {
        // Arrange
        TestHttpMessageHandler handler = new(req =>
        {
            if (req.RequestUri?.AbsolutePath == "/accounts")
            {
                AccountDto[] accounts = new[] { new AccountDto { AccountId = 1, FirstName = "Jane", LastName = "Doe" } };
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(accounts)
                };
            }
            if (req.RequestUri?.AbsolutePath == "/accounts/1/meter-readings")
            {
                MeterReadingDto[] readings =
                {
                    new MeterReadingDto { AccountId = 1, MeterReadingDateTime = new DateTime(2024,1,2,3,4,5), MeterReadValue = 12345 }
                };
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(readings)
                };
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });
        HttpClient client = new(handler) { BaseAddress = new Uri("http://localhost") };
        Services.AddSingleton(client);
        IRenderedComponent<Home> cut = RenderComponent<Home>();

        // Act
        await cut.InvokeAsync(() => cut.Find("tbody tr").Click());

        // Assert
        FieldInfo? meterReadingsField = typeof(Home).GetField("meterReadings", BindingFlags.NonPublic | BindingFlags.Instance);
        cut.WaitForAssertion(() =>
        {
            IList<MeterReadingDto>? readings = meterReadingsField?.GetValue(cut.Instance) as IList<MeterReadingDto>;
            Assert.NotNull(readings);
            Assert.Single(readings!);
            Assert.Equal(12345, readings![0].MeterReadValue);
        });
    }
}