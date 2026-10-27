namespace ModBuilderCLI.Core.Misc;

public class EmptyResponseException(string message) : InvalidOperationException(message);