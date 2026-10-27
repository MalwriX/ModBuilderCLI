using ModBuilderCLI.Core.API;
using ModBuilderCLI.Core.Commands;
using ModBuilderCLI.Core.IO;
using ModBuilderCLI.Core.Misc;
using Spectre.Console;

CacheConfigHandler.InitDirs();
PreStartFetch.Fetch();

Commands c = new Commands();
CommandsRegistry.Register(ref c);

AnsiConsole.MarkupLine(Runtime.FigletName);
AnsiConsole.MarkupLine($"[gray]ModBuilder CLI {Runtime.Version}[/]");
AnsiConsole.MarkupLine("[gray]Made By [/][cyan]MalwriX[/]");

while (Runtime.Running)
{
    Console.WriteLine("");
    AnsiConsole.Markup("[white]|[/][blue]ModBuilder[/][white]|[/] [lime]$[/] ");
    c.TryHandle(Console.ReadLine());
}