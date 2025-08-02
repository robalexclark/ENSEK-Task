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
        HttpResponseMessage response = await client.PostAsync("/api/meter-readings/meter-reading-uploads", content);
        string body = await response.Content.ReadAsStringAsync();
        MeterReadingUploadResult result = JsonSerializer.Deserialize<MeterReadingUploadResult>(body)!;

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(2, result.Successful);
        Assert.Equal(0, result.Failed);
    }


    [Fact]
    public async Task Upload_InvalidFile_ReturnsUnprocessable()
    {
        // Arrange
        string csv = "AccountId,MeterReadingDateTime,MeterReadValue\n" +
                  "9999,16/05/2019 09:24,ABCDE\n";

        using HttpContent content = CreateCsvContent(csv);

        // Act
        HttpResponseMessage response = await client.PostAsync("/api/meter-readings/meter-reading-uploads", content);
        string body = await response.Content.ReadAsStringAsync();
        MeterReadingUploadResult result = JsonSerializer.Deserialize<MeterReadingUploadResult>(body)!;

        // Assert
        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal(0, result.Successful);
        Assert.Equal(1, result.Failed);
    }
}