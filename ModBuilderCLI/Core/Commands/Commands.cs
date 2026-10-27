namespace ModBuilderCLI.Core.Commands;

public class Commands
{
    public List<Handler> CommandsList;
    public Commands()
    {
        CommandsList = new();
    }
    
    public void Add(Handler handler) => CommandsList.Add(handler);
    
    public void Remove(Handler handler) => CommandsList.Remove(handler);

    public void TryHandle(string input)
    {
        if(input.IsWhiteSpace()) return;
        string name = input.Split(' ')[0];
        bool exists = CommandsList.Exists(c => c.IsOfName(name));
        if (!exists)
        {
            Console.WriteLine($"Unknown command: {name}");
            return;
        }
        CommandsList.Find(c => c.IsOfName(name))?.Handle(input);
    }
}