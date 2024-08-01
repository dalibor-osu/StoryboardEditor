using System.Text;

namespace StoryboardEditor.Types.OptionalValue;

public sealed class OptionalError
{
    public OptionalErrorType ErrorType { get; init; }
    public string Message { get; init; } = string.Empty;
    public OptionalError? InnerError { get; init; }

    public OptionalError(OptionalErrorType errorType)
    {
        ErrorType = errorType;
    }
    
    public OptionalError(OptionalErrorType errorType, string message)
    {
        ErrorType = errorType;
        Message = message;
    }
    
    public OptionalError(OptionalErrorType errorType, string message, OptionalError innerError)
    {
        ErrorType = errorType;
        Message = message;
        InnerError = innerError;
    }

    public OptionalError(string message, OptionalError innerError)
    {
        ErrorType = innerError.ErrorType;
        Message = message;
        InnerError = innerError;
    }
    
    public override string ToString()
    {
        var builder = new StringBuilder(GetPrintableMessage());
        if (InnerError != null)
        {
            builder.AppendLine($"\t{InnerError.ToString()}");
        }

        return builder.ToString();
    }

    private string GetPrintableMessage()
    {
        return $"[{ErrorType}] {Message}";
    }
}