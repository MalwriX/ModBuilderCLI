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

public record ModrinthVersion(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("project_id")] string ProjectId,
    [property: JsonPropertyName("author_id")] string AuthorId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("version_number")] string VersionNumber,
    [property: JsonPropertyName("version_type")] string VersionType,      // release | beta | alpha
    [property: JsonPropertyName("game_versions")] List<string> GameVersions,
    [property: JsonPropertyName("loaders")] List<string> Loaders,
    [property: JsonPropertyName("environment")] string? Environment, // client | server | c+s
    [property: JsonPropertyName("featured")] bool Featured,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("requested_status")] string? RequestedStatus,
    [property: JsonPropertyName("date_published")] DateTimeOffset DatePublished,
    [property: JsonPropertyName("downloads")] int Downloads,
    [property: JsonPropertyName("changelog_url")] string? ChangelogUrl,
    [property: JsonPropertyName("files")] List<ModrinthFile> Files,
    [property: JsonPropertyName("dependencies")] List<ModrinthDependency> Dependencies,
    [property: JsonPropertyName("changelog")] string? Changelog = null);    // absent with include_changelog=false

public record ModrinthFile(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("hashes")] ModrinthHashes Hashes,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("filename")] string Filename,
    [property: JsonPropertyName("primary")] bool Primary,
    [property: JsonPropertyName("size")] long Size,
    [property: JsonPropertyName("file_type")] string? FileType);

public record ModrinthHashes(
    [property: JsonPropertyName("sha1")] string Sha1,
    [property: JsonPropertyName("sha512")] string Sha512);

public record ModrinthDependency(
    [property: JsonPropertyName("version_id")] string? VersionId,
    [property: JsonPropertyName("project_id")] string? ProjectId,
    [property: JsonPropertyName("file_name")] string? FileName,
    [property: JsonPropertyName("dependency_type")] string DependencyType);