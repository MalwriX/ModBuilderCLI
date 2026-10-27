using System.Net;
using System.Text.Json;
using ModBuilderCLI.Core.IO;
using ModBuilderCLI.Core.Misc;

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
    public static async Task<string> SearchModrinth(string query)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.modrinth.com/v2/search?query={query}");
        var response = await HttpClient.SendAsync(request);
        
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
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