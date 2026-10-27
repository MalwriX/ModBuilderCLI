using ModBuilderCLI.Core.API;
using ModBuilderCLI.Core.IO;
using ModBuilderCLI.Core.Misc;
using Spectre.Console;

namespace ModBuilderCLI.Core.Commands;

public class CommandsRegistry
{
    public static Dictionary<string, string> CommandsInfo = new();

    public static void Register(ref Commands c)
    {
        //help
        var help = Cmd.Literal("help")
            .Then(Cmd.GreedyString("command").Executes(ctx =>
            {
                var cmd = ctx.GetValueOrDefault("command") ?? "";
                if (CommandsInfo.TryGetValue(cmd, out var info))
                    Console.WriteLine($"{cmd}: {info}");
                else
                    Console.WriteLine($"Unknown command: {cmd}");

                ;
            })).Executes(ctx =>
            {
                Console.WriteLine("Available commands:");
                foreach (var kv in CommandsInfo)
                    Console.WriteLine($"  {kv.Key}");
            });

        c.Add(help);
        CommandsInfo["help"] =
            "Usage: help <command / empty> \n Get the list of available commands, or info about a specific command.";

        //exit
        var exit = Cmd.Literal("exit").Then(Cmd.GreedyString("void")).Executes(ctx => { Runtime.Running = false; });

        c.Add(exit);
        CommandsInfo["exit"] = "Exit the program.";

        //clear
        var clear = Cmd.Literal("clear").Then(Cmd.GreedyString("void")).Executes(ctx =>
        {
            Console.Clear();
            AnsiConsole.MarkupLine(Runtime.FigletName);
            AnsiConsole.MarkupLine($"[gray]ModBuilder CLI {Runtime.Version}[/]");
            AnsiConsole.MarkupLine("[gray]Made By [/][cyan]MalwriX[/]");
        });

        c.Add(clear);
        CommandsInfo["clear"] = "Clear the console.";

        //search
        var search = Cmd.Literal("search")
            .Then(Cmd.Argument("provider")
                .Then(Cmd.GreedyString("query")
                    .Executes(ctx =>
                    {
                        string provider = ctx["provider"];

                        Provider? recordedProvider = null;
                        if (Runtime.CfAliases.Contains(provider)) recordedProvider = Provider.Curse;
                        else if
                            (Runtime.ModrinthAliases.Contains(provider)) recordedProvider = Provider.Modrinth;
                        else if
                            (Runtime.BothAliases.Contains(provider)) recordedProvider = Provider.Both;
                        else
                        {
                            Console.WriteLine($"Invalid Provider Argument: {provider}");
                            return;
                        }

                        switch (recordedProvider)
                        {
                            case Provider.Modrinth:
                            {
                                string result = ApiHandler.SearchModrinth(ctx["query"]).GetAwaiter().GetResult();
                                Console.WriteLine(result);
                            }
                                break;

                            case Provider.Curse:
                            {
                                Console.WriteLine("CF Not Yet Supported");
                            }
                                break;

                            case Provider.Both:
                            {
                                string result = ApiHandler.SearchModrinth(ctx["query"]).GetAwaiter().GetResult();
                                Console.WriteLine(result);
                                Console.WriteLine("CF Not Yet Supported");
                            }
                                break;
                        }
                    })));

        c.Add(search);
        CommandsInfo["search"] =
            "Usage: search <provider> <query> \n Searches either CF, Modrinth, or both, for mods based on a query";

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
                .Executes(ctx =>
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
                .Executes(ctx =>
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
                .Executes(ctx =>
                {
                    if(File.Exists("cache/versions.json")) File.Delete("cache/versions.json");
                    CacheConfigHandler.GetGameVersionTags();
                }))
            .Then(Cmd.Literal("set-enforce")
                .Executes(ctx =>
                {
                    var prompt = new SelectionPrompt<bool>()
                        .Title("Enforce versions")
                        .AddChoices(true, false);
                    Runtime.OptionEnforceVersions = AnsiConsole.Prompt(prompt);
                }))
            .Then(Cmd.Literal("include-snapshots")
                .Executes(ctx =>
                {
                    var prompt = new SelectionPrompt<bool>()
                        .Title("Include Snapshot Versions")
                        .AddChoices(true, false);
                    Runtime.IncludeSnapshots = AnsiConsole.Prompt(prompt);
                    Runtime.Versions = ApiHandler.GetGameVersions();
                    
                    Runtime.EnforcedVersions.RemoveAll(v => !Runtime.Versions.Contains(v));
                }))
            .Then(Cmd.Literal("clear")
                .Executes(ctx =>
                {
                    Runtime.EnforcedVersions.Clear();
                }));

        c.Add(version);
        CommandsInfo["version"] = "Various commands for controlling version-related settings";

        var loader = Cmd.Literal("loader")
            .Then(Cmd.Literal("get")
                .Executes(ctx =>
        {
            Console.WriteLine($"Enforcing Loaders: {Runtime.OptionEnforceLoaders}");
            Console.WriteLine($"Selected Loaders:");
            foreach (var loader in Runtime.EnforcedLoaders)
            {
                Console.WriteLine(loader);
            }
        }))
            .Then(Cmd.Literal("set")
                .Executes(ctx =>
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
                .Executes(ctx =>
            {
                var prompt = new SelectionPrompt<bool>()
                    .Title("Enforce loaders")
                    .AddChoices(true, false);
                Runtime.OptionEnforceLoaders = AnsiConsole.Prompt(prompt);
            }))
            .Then(Cmd.Literal("invalidate-cache")
                .Executes(ctx =>
                {
                    if(File.Exists("cache/loaders.json")) File.Delete("cache/loaders.json");
                    CacheConfigHandler.GetLoaderTags();
                }))
            .Then(Cmd.Literal("clear")
                .Executes(ctx =>
                {
                    Runtime.EnforcedLoaders.Clear();
                }));

        c.Add(loader);
        CommandsInfo["loader"] = "Various commands for controlling mod loader-related settings";
    }
}

internal enum Provider
{
    Curse,
    Modrinth,
    Both
}