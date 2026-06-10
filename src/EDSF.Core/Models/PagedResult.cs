using System.Text.Json.Serialization;

namespace EDSF.Core.Models;

public record PagedResult<T>
{
    [JsonPropertyName("items")]
    public List<T> Items { get; init; } = new();

    [JsonPropertyName("total")]
    public int Total { get; init; }

    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; init; }
}
