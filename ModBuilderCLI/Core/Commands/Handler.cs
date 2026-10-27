namespace ModBuilderCLI.Core.Commands;

public class Handler
{
    public string Name { get; }
    public bool IsLiteral { get; }

    public bool IsGreedyString { get; }
    public List<Handler> Children { get; } = new();
    private Action<Dictionary<string, string>>? _executor;

    public Handler(string name, bool isLiteral, bool isGreedyString)
    {
        Name = name;
        IsLiteral = isLiteral;
        IsGreedyString = isGreedyString;
    }

    public Handler Then(Handler child)
    {
        Children.Add(child);
        return this;
    }

    public Handler Executes(Action<Dictionary<string, string>> f)
    {
        _executor = f;
        return this;
    }

    public void Handle(string input)
    {
        var tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0 || !IsOfName(tokens[0]))
        {
            Console.WriteLine("Unknown command");
            return;
        }

        Run(tokens, 1, new Dictionary<string, string>());
    }

    private void Run(string[] tokens, int i, Dictionary<string, string> ctx)
    {
        if (i == tokens.Length)
        {
            Execute(ctx);
            return;
        }

        string token = tokens[i];
        var next = Children.Find(c => c.IsLiteral && c.Name == token)
                   ?? Children.Find(c => !c.IsLiteral);

        if (next == null)
        {
            Console.WriteLine($"invalid argument: {token}");
            return;
        }

        if (next.IsGreedyString)
        {
            ctx[next.Name] = string.Join(' ', tokens[i..]);
            next.Execute(ctx);
            return;
        }

        if (!next.IsLiteral) ctx[next.Name] = token;
        next.Run(tokens, i + 1, ctx);
    }
    
    private void Execute(Dictionary<string, string> ctx)
    {
        if (_executor != null) _executor(ctx);
        else Console.WriteLine("incomplete command");
    }
    
    public bool IsOfName(string name) => Name == name;
}

static class Cmd
{
    public static Handler Literal(string name) => new(name, true, false);
    public static Handler Argument(string name) => new(name, false, false);
    public static Handler GreedyString(string name) => new(name, false, true);
}