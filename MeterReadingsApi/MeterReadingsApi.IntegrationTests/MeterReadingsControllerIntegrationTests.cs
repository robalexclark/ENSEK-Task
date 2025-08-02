using MeterReadingsApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace MeterReadingsApi.IntegrationTests;

[ExcludeFromCodeCoverage]
public class MeterReadingsControllerIntegrationTests : IClassFixture<TestApiFactory>
{
    private readonly HttpClient client;

    public MeterReadingsControllerIntegrationTests(TestApiFactory factory)
    {
        client = factory.CreateClient();
    }

    private static HttpContent CreateCsvContent(string csv)
    {
        MultipartFormDataContent content = new MultipartFormDataContent();
        byte[] bytes = Encoding.UTF8.GetBytes(csv);
        ByteArrayContent fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
        content.Add(fileContent, "file", "readings.csv");
        return content;
    }

    [Fact]
    public async Task Upload_ValidFile_ReturnsCreated()
    {
        // Arrange
        string csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                  "2344,16/05/2019 09:24,00123\n" +
                  "2233,17/05/2019 12:00,00456\n";

        using HttpContent content = CreateCsvContent(csv);

        // Act
        HttpResponseMessage response = await client.PostAsync("/meter-reading-uploads", content);
        string body = await response.Content.ReadAsStringAsync();
        MeterReadingUploadResult result = JsonSerializer.Deserialize<MeterReadingUploadResult>(body)!;

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(2, result.Successful);
        Assert.Equal(0, result.Failed);
        Assert.Empty(result.Failures);
    }

    [Fact]
    public async Task Upload_InvalidFile_ReturnsUnprocessable()
    {
        // Arrange
        string csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                  "9999,16/05/2019 09:24,ABCDE\n";

        using HttpContent content = CreateCsvContent(csv);

        // Act
        HttpResponseMessage response = await client.PostAsync("/meter-reading-uploads", content);
        string body = await response.Content.ReadAsStringAsync();
        MeterReadingUploadResult result = JsonSerializer.Deserialize<MeterReadingUploadResult>(body)!;

        // Assert
        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(0, result.Successful);
        Assert.Equal(1, result.Failed);
        MeterReadingUploadFailure failure = Assert.Single(result.Failures);
        Assert.Equal(2, failure.RowNumber);
        Assert.Equal(9999, failure.AccountId);
    }

    [Fact]
    public async Task Upload_MissingFile_ReturnsBadRequest()
    {
        // Arrange
        MultipartFormDataContent content = new MultipartFormDataContent();

        // Act
        HttpResponseMessage response = await client.PostAsync("/meter-reading-uploads", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Upload_InvalidHeaders_ReturnsBadRequest()
    {
        // Arrange - missing MeterReadValue header
        string csv = "AccountId,MeterReadingDateTime\n" +
                     "2344,16/05/2019 09:24\n";

        using HttpContent content = CreateCsvContent(csv);

        // Act
        HttpResponseMessage response = await client.PostAsync("/meter-reading-uploads", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Upload_FileWithBlankRow_ReturnsBadRequest()
    {
        // Arrange - file contains a blank line
        string csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                     "2344,16/05/2019 09:24,00123\n" +
                     "\n" +
                     "2233,17/05/2019 12:00,00456\n";

        using HttpContent content = CreateCsvContent(csv);

        // Act
        HttpResponseMessage response = await client.PostAsync("/meter-reading-uploads", content);
        string body = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("blank rows", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Upload_FileWithUnknownAccount_IgnoresReading()
    {
        // Arrange
        string csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                     "4534,19/05/2019 09:24,00123\n" +
                     "9999,17/05/2019 12:00,00456\n";

        using HttpContent content = CreateCsvContent(csv);

        // Act
        HttpResponseMessage response = await client.PostAsync("/meter-reading-uploads", content);
        string body = await response.Content.ReadAsStringAsync();
        MeterReadingUploadResult result = JsonSerializer.Deserialize<MeterReadingUploadResult>(body)!;

        // Assert
        Assert.Equal((HttpStatusCode)207, response.StatusCode);
        Assert.Equal(1, result.Successful);
        Assert.Equal(1, result.Failed);
        MeterReadingUploadFailure failure2 = Assert.Single(result.Failures);
        Assert.Equal(3, failure2.RowNumber);
        Assert.Equal(9999, failure2.AccountId);
    }

    [Fact]
    public async Task GetByAccountId_ReturnsReadings()
    {
        // Arrange - upload a reading first
        string csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                     "2344,18/05/2019 09:24,00123\n";

        using HttpContent content = CreateCsvContent(csv);
        HttpResponseMessage upload = await client.PostAsync("/meter-reading-uploads", content);
        upload.EnsureSuccessStatusCode();

        // Act
        HttpResponseMessage response = await client.GetAsync("/accounts/2344/meter-readings");
        response.EnsureSuccessStatusCode();
        string body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("\"account\":", body, StringComparison.OrdinalIgnoreCase);
        List<MeterReadingDto> readings = JsonSerializer.Deserialize<List<MeterReadingDto>>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        // Assert
        Assert.NotEmpty(readings);
        MeterReadingDto reading = readings.First();
        Assert.Equal(2344, reading.AccountId);
        Assert.Equal(123, reading.MeterReadValue);
    }

    [Fact]
    public async Task GetByAccountId_ReturnsNoContent_When_NoReadings()
    {
        // Act
        HttpResponseMessage response = await client.GetAsync("/accounts/2352/meter-readings");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetByAccountId_ReturnsNotFound_When_AccountMissing()
    {
        // Act
        HttpResponseMessage response = await client.GetAsync("/accounts/9999/meter-readings");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}