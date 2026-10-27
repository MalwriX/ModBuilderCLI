using System.Text.Json.Serialization;

namespace ModBuilderCLI.Core.RecordHolder;

public class ModrinthRecordHolder
{
}

public record ModrinthSearchResponse(
    [property: JsonPropertyName("hits")] ModrinthHit[] Hits,
    [property: JsonPropertyName("total_hits")]
    int TotalHits
);

public record ModrinthHit(
    [property: JsonPropertyName("project_id")] string ProjectId,
    [property: JsonPropertyName("slug")] string Slug,
    [property: JsonPropertyName("author")] string Author,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("versions")] string[] Versions
);

public record LoaderTag(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("supported_project_types")] string[] SupportedProjectTypes);

public record GameVersionTag(
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("version_type")] string VersionType,
    [property: JsonPropertyName("date")] DateTimeOffset Date,
    [property: JsonPropertyName("major")] bool Major);