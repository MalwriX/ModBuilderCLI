using ModBuilderCLI.Core.Misc;

namespace ModBuilderCLI.Core.API;

public class PreStartFetch
{
    public static void Fetch()
    {
        Runtime.Versions = ApiHandler.GetGameVersions();
        //File.WriteAllLines("cache/versions.json", Runtime.Versions);
        Runtime.Loaders = ApiHandler.GetLoaders();
    }
}