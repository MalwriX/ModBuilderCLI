namespace ModBuilderCLI.Core.Misc;

public static class Runtime
{
    public const string FigletName = "[teal] \n" +
                                     "██      ██   ██████   ██████        ████ ██     ██████\n" +
                                     "████  ████ ██      ██ ██    ██    ██     ██       ██\n" +
                                     "██  ██  ██ ██      ██ ██    ██    ██     ██       ██\n" +
                                     "██      ██   ██████   ██████        ████ ██████ ██████\n" +
                                     "[/]";

    public const string Version = "v1.0";

    public static bool Running = true;

    public static List<string> Loaders = new();
    public static List<string> Versions = new();
    
    public static string[] CfAliases =
    [
        "cf", "curse", "cforge", "curseforge", "forge", 
        "c-forge", "curse_forge", "cfacts", "c"
    ];

    public static string[] ModrinthAliases =
    [
        "mod", "modrinth", "rinth", "mr", "m", 
        "mod-rinth", "modr", "rinth-mod", "mod-r"
    ];

    public static string[] BothAliases =
    [
        "both", "all", "bo", "b", 
        "every", "everything", "combined", "combo", "a"
    ];

    public static bool OptionEnforceLoaders = false;
    public static List<string> EnforcedLoaders = new();
    public static bool OptionEnforceVersions = false;
    public static List<string> EnforcedVersions = new();

    public static bool IncludeSnapshots = false;
}