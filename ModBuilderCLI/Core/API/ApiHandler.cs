using System.Net;
using System.Text.Json;
using ModBuilderCLI.Core.IO;
using ModBuilderCLI.Core.Misc;
using ModBuilderCLI.Core.RecordHolder;

namespace ModBuilderCLI.Core.API;

public class ApiHandler
{
    private static readonly HttpClient HttpClient = CreateClient();

    private static HttpClient CreateClient()
    {
        var client = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        });

        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            $"ModBuilderCLI/{Runtime.Version} (https://github.com/MalwriX/ModBuilderCLI)");

        return client;
    }

    public static async Task<ModrinthSearchResponse> SearchModrinth(string query, int limit = 20)
    {
        var url = $"https://api.modrinth.com/v2/search?query={Uri.EscapeDataString(query)}&limit={limit}";
        var json = await HttpClient.GetStringAsync(url);

        try
        {
            return JsonSerializer.Deserialize<ModrinthSearchResponse>(json)
                           ?? throw new EmptyResponseException("Modrinth returned an empty search response.");
        }
        catch (Exception e)
        {
            return new ModrinthSearchResponse([], 0);
        }
        
        
    }

    public static async Task<ModrinthVersion[]> GetProjectVersionsModrinth(string id)
    {
        var response = await HttpClient.GetAsync($"https://api.modrinth.com/v2/project/{id}/version");
        var json = await response.Content.ReadAsStringAsync();
        try
        {
            return JsonSerializer.Deserialize<ModrinthVersion[]>(json) ??
                   throw new EmptyResponseException("Modrinth returned an empty version list.");
        }
        catch (Exception e)
        {
            return [];
        }
    }

    public static async Task<List<T>> GetTagsAsync<T>(string tag)
    {
        var json = await HttpClient.GetStringAsync($"https://api.modrinth.com/v2/tag/{tag}");
        return JsonSerializer.Deserialize<List<T>>(json) ?? [];
    }

    public static List<string> GetGameVersions() =>
        CacheConfigHandler.GetGameVersionTags()
            .Where(v => Runtime.IncludeSnapshots || v.VersionType == "release")
            .OrderByDescending(v => v.Date)
            .Select(v => v.Version)
            .ToList();

    public static List<string> GetLoaders() =>
        CacheConfigHandler.GetLoaderTags()
            .Where(l => l.SupportedProjectTypes.Contains("mod"))
            .Select(l => l.Name)
            .ToList();
}