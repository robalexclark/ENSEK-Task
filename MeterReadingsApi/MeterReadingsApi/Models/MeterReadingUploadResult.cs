using System.Text.Json.Serialization;

namespace MeterReadingsApi.Models;

/// <summary>
/// Represents the outcome of a meter reading upload including counts and failure details.
/// </summary>
[Serializable]
public readonly record struct MeterReadingUploadResult(
    [property: JsonPropertyName("successful")] int Successful,
    [property: JsonPropertyName("failed")] int Failed,
    [property: JsonPropertyName("failures")] IReadOnlyList<MeterReadingUploadFailure> Failures);