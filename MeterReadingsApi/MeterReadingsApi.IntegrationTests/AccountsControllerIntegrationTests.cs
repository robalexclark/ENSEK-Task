using MeterReadingsApi.DataModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Xunit;

namespace MeterReadingsApi.IntegrationTests;

[ExcludeFromCodeCoverage]
public class AccountsControllerIntegrationTests : IClassFixture<TestApiFactory>
{
    private readonly HttpClient client;

    public AccountsControllerIntegrationTests(TestApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsAccounts()
    {
        // Act
        HttpResponseMessage response = await client.GetAsync("/accounts");
        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync();
        List<Account> accounts = JsonSerializer.Deserialize<List<Account>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        // Assert
        Assert.NotEmpty(accounts);
    }
}