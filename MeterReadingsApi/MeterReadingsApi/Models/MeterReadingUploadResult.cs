using System.Text.Json.Serialization;

namespace MeterReadingsApi.Models
{
    [Serializable]
    public readonly record struct MeterReadingUploadResult(
        [property: JsonPropertyName("successful")] int Successful,
        [property: JsonPropertyName("failed")] int Failed);
}