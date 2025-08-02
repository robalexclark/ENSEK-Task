using MeterReadingsApi.Shared;
using MeterReadingsBlazorClient.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

public partial class HomeTests : BlazoriseTestBase
{
    [Fact]
    public void Home_LoadsAccountsOnInit()
    {
        var handler = new TestHttpMessageHandler(req =>
        {
            if (req.RequestUri?.AbsolutePath == "/accounts")
            {
                var accounts = new[] { new AccountDto { AccountId = 1, FirstName = "Jane", LastName = "Doe" } };
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(accounts)
                };
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        Services.AddSingleton(client);

        var cut = RenderComponent<Home>();

        var accountsField = typeof(Home).GetField("accounts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var accounts = accountsField?.GetValue(cut.Instance) as IList<AccountDto>;
        Assert.NotNull(accounts);
        Assert.Single(accounts!);
        Assert.Equal("Jane", accounts![0].FirstName);
    }

    [Fact]
    public async Task Home_LoadsMeterReadingsWhenAccountSelected()
    {
        var handler = new TestHttpMessageHandler(req =>
        {
            if (req.RequestUri?.AbsolutePath == "/accounts")
            {
                var accounts = new[] { new AccountDto { AccountId = 1, FirstName = "Jane", LastName = "Doe" } };
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(accounts)
                };
            }
            if (req.RequestUri?.AbsolutePath == "/accounts/1/meter-readings")
            {
                var readings = new[]
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
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        Services.AddSingleton(client);

        var cut = RenderComponent<Home>();
        var account = new AccountDto { AccountId = 1, FirstName = "Jane", LastName = "Doe" };
        var method = typeof(Home).GetMethod("OnAccountSelected", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
        await cut.InvokeAsync(() => (Task)method.Invoke(cut.Instance, new object[] { account })!);

        var meterReadingsField = typeof(Home).GetField("meterReadings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var readings = meterReadingsField?.GetValue(cut.Instance) as IList<MeterReadingDto>;
        Assert.NotNull(readings);
        Assert.Single(readings!);
        Assert.Equal(12345, readings![0].MeterReadValue);
    }
}