using System.Text.Json;
using ModBuilderCLI.Core.API;
using ModBuilderCLI.Core.RecordHolder;

namespace ModBuilderCLI.Core.IO;

public static class CacheConfigHandler
{
    public static void InitDirs()
    {
        if(!Directory.Exists("cache"))
        {
            Directory.CreateDirectory("cache");
        }
        if(!Directory.Exists("config")) 
        {
            Directory.CreateDirectory("config");
        }
    }
    private static List<T> CacheOrFetch<T>(string path, string tag)
    {
        List<T>? cached = null;

        if (File.Exists(path))
        {
            try
            {
                var json = File.ReadAllText(path);
                if (!string.IsNullOrWhiteSpace(json))
                    cached = JsonSerializer.Deserialize<List<T>>(json);
            }
            catch (JsonException) { }
            catch (IOException) { }
        }

        bool fresh = cached is { Count: > 0 }
                     && File.GetLastWriteTimeUtc(path) > DateTime.UtcNow.AddDays(-1);
        if (fresh) return cached!;

        try
        {
            var result = ApiHandler.GetTagsAsync<T>(tag).GetAwaiter().GetResult();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, JsonSerializer.Serialize(result));
            return result;
        }
        catch (HttpRequestException) when (cached is { Count: > 0 })
        {
            return cached;
        }
    }

    public static List<GameVersionTag> GetGameVersionTags() =>
        CacheOrFetch<GameVersionTag>("cache/versions.json", "game_version");

    public static List<LoaderTag> GetLoaderTags() =>
        CacheOrFetch<LoaderTag>("cache/loaders.json", "loader");
}