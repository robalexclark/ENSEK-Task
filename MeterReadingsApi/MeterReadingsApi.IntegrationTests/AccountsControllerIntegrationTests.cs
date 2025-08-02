using MeterReadingsApi.Shared;
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
        HttpResponseMessage response = await client.GetAsync("/accounts");
        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync();
        List<AccountDto> accounts = JsonSerializer.Deserialize<List<AccountDto>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        Assert.NotEmpty(accounts);
    }

    [Fact]
    public async Task Get_DoesNotReturnMeterReadings()
    {
        HttpResponseMessage response = await client.GetAsync("/accounts");
        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync();

        using JsonDocument doc = JsonDocument.Parse(body);
        foreach (JsonElement account in doc.RootElement.EnumerateArray())
        {
            Assert.False(account.TryGetProperty("meterReadings", out _));
        }
    }
}