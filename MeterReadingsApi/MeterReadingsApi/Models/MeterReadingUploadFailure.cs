using System.Text.Json.Serialization;

namespace MeterReadingsApi.Models;

/// <summary>
/// Represents a single failed meter reading upload including the row number and reason.
/// </summary>
[Serializable]
public readonly record struct MeterReadingUploadFailure(
    [property: JsonPropertyName("rowNumber")] int RowNumber,
    [property: JsonPropertyName("reason")] string Reason);

