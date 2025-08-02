using System.Text.Json.Serialization;

namespace MeterReadingsApi.Models;

/// <summary>
/// Represents a single failed meter reading upload including the row number,
/// optional account identifier, and reason.
/// </summary>
[Serializable]
public readonly record struct MeterReadingUploadFailure(
    [property: JsonPropertyOrder(1), JsonPropertyName("rowNumber")] int RowNumber,
    [property: JsonPropertyOrder(2), JsonPropertyName("accountId"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? AccountId,
    [property: JsonPropertyOrder(3), JsonPropertyName("reason")] string Reason);

