using ModBuilderCLI.Core.API;
using ModBuilderCLI.Core.IO;
using ModBuilderCLI.Core.Misc;
using ModBuilderCLI.Core.RecordHolder;
using Spectre.Console;

namespace ModBuilderCLI.Core.Commands;

public static class CommandsRegistry
{
    private static readonly Dictionary<string, string> CommandsInfo = new();

    public static void Register(ref Commands c)
    {
        //help
        var help = Cmd.Literal("help")
            .Then(Cmd.GreedyString("command").Executes(ctx =>
            {
                var cmd = ctx.GetValueOrDefault("command") ?? "";
                Console.WriteLine(CommandsInfo.TryGetValue(cmd, out var info)
                    ? $"{cmd}: {info}"
                    : $"Unknown command: {cmd}");
            })).Executes(_ =>
            {
                Console.WriteLine("Available commands:");
                foreach (var kv in CommandsInfo)
                    Console.WriteLine($"  {kv.Key}");
            });

        c.Add(help);
        CommandsInfo["help"] =
            "Usage: help <command / empty> \n Get the list of available commands, or info about a specific command.";

        //exit
        var exit = Cmd.Literal("exit").Then(Cmd.GreedyString("void")).Executes(_ => { Runtime.Running = false; });

        c.Add(exit);
        CommandsInfo["exit"] = "Exit the program.";

        //clear
        var clear = Cmd.Literal("clear").Then(Cmd.GreedyString("void")).Executes(_ =>
        {
            Console.Clear();
            AnsiConsole.MarkupLine(Runtime.FigletName);
            AnsiConsole.MarkupLine($"[gray]ModBuilder CLI {Runtime.Version}[/]");
            AnsiConsole.MarkupLine("[gray]Made By [/][cyan]MalwriX[/]");
        });

        c.Add(clear);
        CommandsInfo["clear"] = "Clear the console.";
        
        var config = Cmd.Literal("config").Then(Cmd.Argument("configOption").Executes(ctx =>
        {
            string option = ctx["configOption"];
            switch (option)
            {
                case "help":
                {
                }
                    break;
                case "debug":
                {
                    Console.WriteLine($"Enforce Loaders: {Runtime.OptionEnforceLoaders}");
                    string enf = "";
                    foreach (var loader in Runtime.EnforcedLoaders)
                    {
                        enf += $"{loader}, ";
                    }

                    Console.WriteLine($"Enforced Loaders: {enf}");
                }
                    break;
                default:
                {
                    Console.WriteLine(
                        $"Unknown option: {option}. type 'config help' to get a list of available options");
                }
                    break;
            }
        }));

        c.Add(config);
        CommandsInfo["config"] = "Usage: config <param> \n Configure various settings of the tool";

        var version = Cmd.Literal("version")
            .Then(Cmd.Literal("set")
                .Executes(_ =>
                {
                    var prompt = new MultiSelectionPrompt<string>()
                        .Title("Select versions")
                        .InstructionsText("[grey](^/v move, [blue]space[/] toggle, [green]enter[/] accept)[/]")
                        .AddChoices(Runtime.Versions.ToArray());

                    foreach (var version in Runtime.EnforcedVersions)
                        prompt.Select(version);

                    Runtime.EnforcedVersions = AnsiConsole.Prompt(prompt);
                }))
            .Then(Cmd.Literal("get")
                .Executes(_ =>
                {
                    Console.WriteLine($"Enforcing Versions: {Runtime.OptionEnforceVersions}");
                    Console.WriteLine($"Snapshots Enabled: {Runtime.IncludeSnapshots}");
                    Console.WriteLine("Selected Versions:");
                    foreach (var version in Runtime.EnforcedVersions)
                    {
                        Console.WriteLine(version);
                    }
                }))
            .Then(Cmd.Literal("invalidate-cache")
                .Executes(_ =>
                {
                    if (File.Exists("cache/versions.json")) File.Delete("cache/versions.json");
                    CacheConfigHandler.GetGameVersionTags();
                }))
            .Then(Cmd.Literal("set-enforce")
                .Executes(_ =>
                {
                    var prompt = new SelectionPrompt<bool>()
                        .Title("Enforce versions")
                        .AddChoices(true, false);
                    Runtime.OptionEnforceVersions = AnsiConsole.Prompt(prompt);
                }))
            .Then(Cmd.Literal("include-snapshots")
                .Executes(_ =>
                {
                    var prompt = new SelectionPrompt<bool>()
                        .Title("Include Snapshot Versions")
                        .AddChoices(true, false);
                    Runtime.IncludeSnapshots = AnsiConsole.Prompt(prompt);
                    Runtime.Versions = ApiHandler.GetGameVersions();

                    Runtime.EnforcedVersions.RemoveAll(v => !Runtime.Versions.Contains(v));
                }))
            .Then(Cmd.Literal("clear")
                .Executes(_ => { Runtime.EnforcedVersions.Clear(); }));

        c.Add(version);
        CommandsInfo["version"] = "Various commands for controlling version-related settings";

        var loader = Cmd.Literal("loader")
            .Then(Cmd.Literal("get")
                .Executes(_ =>
                {
                    Console.WriteLine($"Enforcing Loaders: {Runtime.OptionEnforceLoaders}");
                    Console.WriteLine($"Selected Loaders:");
                    foreach (var loader in Runtime.EnforcedLoaders)
                    {
                        Console.WriteLine(loader);
                    }
                }))
            .Then(Cmd.Literal("set")
                .Executes(_ =>
                {
                    var prompt = new MultiSelectionPrompt<string>()
                        .Title("Select mod loaders")
                        .InstructionsText("[grey](^/v move, [blue]space[/] toggle, [green]enter[/] accept)[/]")
                        .AddChoices(Runtime.Loaders.ToArray());

                    foreach (var loader in Runtime.EnforcedLoaders)
                        prompt.Select(loader);

                    Runtime.EnforcedLoaders = AnsiConsole.Prompt(prompt);
                }))
            .Then(Cmd.Literal("set-enforce")
                .Executes(_ =>
                {
                    var prompt = new SelectionPrompt<bool>()
                        .Title("Enforce loaders")
                        .AddChoices(true, false);
                    Runtime.OptionEnforceLoaders = AnsiConsole.Prompt(prompt);
                }))
            .Then(Cmd.Literal("invalidate-cache")
                .Executes(_ =>
                {
                    if (File.Exists("cache/loaders.json")) File.Delete("cache/loaders.json");
                    CacheConfigHandler.GetLoaderTags();
                }))
            .Then(Cmd.Literal("clear")
                .Executes(_ => { Runtime.EnforcedLoaders.Clear(); }));

        c.Add(loader);
        CommandsInfo["loader"] = "Various commands for controlling mod loader-related settings";

        var mod = Cmd.Literal("mod")
            .Then(Cmd.Literal("search-all")
                .Then(Cmd.GreedyString("query")
                    .Executes(ctx =>
                    {
                        ModrinthSearchResponse
                            result = ApiHandler.SearchModrinth(ctx["query"]).GetAwaiter().GetResult();
                        var hitArray = result.Hits;

                        foreach (var hit in hitArray)
                        {
                            Console.WriteLine("Current Hit:");
                            Console.WriteLine(
                                $"{hit.Title} by {hit.Author} with Slug of: {hit.Slug} and ProjectId of {hit.ProjectId}");
                            Console.WriteLine($"Versions: {hit.Versions}");
                        }
                    })))
            .Then(Cmd.Literal("search")
                .Then(Cmd.Argument("limit")
                    .Then(Cmd.GreedyString("query").Executes(ctx =>
                    {
                        ModrinthSearchResponse result = ApiHandler
                            .SearchModrinth(ctx["query"], int.TryParse(ctx["limit"], out int e) ? e : 10).GetAwaiter()
                            .GetResult();
                        var hitArray = result.Hits;

                        foreach (var hit in hitArray)
                        {
                            Console.WriteLine("Current Hit:");
                            Console.WriteLine(
                                $"{hit.Title} by {hit.Author} with Slug of: {hit.Slug} and ProjectId of {hit.ProjectId}");
                            string vList = "";
                            foreach (var v in hit.Versions)
                            {
                                vList += $"{v}, ";
                            }

                            Console.WriteLine($"Versions: {vList}");
                        }
                    })))).Then(Cmd.Literal("versions").Then(Cmd.GreedyString("idOrSlug").Executes(ctx =>
            {
                var idOrSlug = ctx["idOrSlug"];
                ModrinthVersion[] v;
                try
                {
                    v = ApiHandler.GetProjectVersionsModrinth(idOrSlug).GetAwaiter().GetResult();
                }
                catch (EmptyResponseException)
                {
                    Console.WriteLine(
                        "Modrinth returned an empty response. Make sure the id | slug is written correctly.");
                    return;
                }

                foreach (var ver in v)
                {
                    string filenames = "";
                    foreach (var file in ver.Files)
                    {
                        filenames += $"{file.Filename}, ";
                    }
                    Console.WriteLine(
                        $"version: Loaders: {string.Join(",", ver.Loaders)}; V. Type: {string.Join(",", ver.VersionType)}; Dependencies: {string.Join(",", ver.Dependencies)}; " +
                        $"Game Versions: {string.Join(",", ver.GameVersions)}; Environment: {string.Join(",", ver.Environment)}; VersionId: {ver.Id}; FileName: {filenames}");
                }
            })));

        c.Add(mod);
        CommandsInfo["mod"] =
            "Usage: \n   - mod search <limit> <query> - search for the topmost <limit> mods \n   - mod search-all <query> - search for all matching mods \n   - mod versions <id|slug> - gathers all versions of a mod \n Various commands for adding, removing, and changing mods";

        var project = Cmd.Literal("project");
    }
}