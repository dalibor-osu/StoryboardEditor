using System.Globalization;
using StoryboardEditor.OsuStoryboard.Enums;
using StoryboardEditor.Types.OptionalValue;

namespace StoryboardEditor.OsuStoryboard.Commands;

public abstract class StoryboardCommand
{
    public Easing Easing { get; init; }
    public int StartTime { get; init; }
    public int EndTime { get; init; }

    protected const int VALUES_OFFSET = 4;
    
    public static Optional<StoryboardCommand> FromLine(string line, bool isChildCommand = false)
    {
        if (line[0] != ' ' && line[0] != '_')
        {
            return new OptionalError(OptionalErrorType.InvalidValue, $"Command line must start with ' ' or '_'. \"{line}\"");
        }

        if (isChildCommand && line[1] != ' ' && line[1] != '_')
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                "Child command line's 2nd char must be ' ' or '_'");
        }

        int offset = isChildCommand ? 2 : 1;
        string trimmedLine = new string(line.Skip(offset).ToArray()).Trim();

        switch (trimmedLine[0])
        {
            case 'F':
                return ConvertToOptionalStoryboardCommand(Fade.FromLine(trimmedLine));
            case 'M':
                return trimmedLine[1] switch
                {
                    ',' => ConvertToOptionalStoryboardCommand(Move.FromLine(trimmedLine)),
                    'X' => ConvertToOptionalStoryboardCommand(MoveX.FromLine(trimmedLine)),
                    'Y' => ConvertToOptionalStoryboardCommand(MoveY.FromLine(trimmedLine)),
                    _ => new OptionalError(OptionalErrorType.InvalidValue,
                        $"Invalid Move command identifier: {trimmedLine[0]}")
                };
            case 'S':
                return ConvertToOptionalStoryboardCommand(Scale.FromLine(trimmedLine));
            case 'V':
                return ConvertToOptionalStoryboardCommand(VectorScale.FromLine(trimmedLine));
            case 'R':
                return ConvertToOptionalStoryboardCommand(Rotate.FromLine(trimmedLine));
            case 'C':
                return ConvertToOptionalStoryboardCommand(Color.FromLine(trimmedLine));
            case 'P':
                return ConvertToOptionalStoryboardCommand(ParameterCommand.FromLine(trimmedLine));
            default:
                return new OptionalError(OptionalErrorType.InvalidValue, $"Unknown command identifier: {trimmedLine[0]}");
        }
    }

    private static Optional<StoryboardCommand> ConvertToOptionalStoryboardCommand<T>(Optional<T> optional) where T : StoryboardCommand => optional.HasError ? optional.Error : optional.Value;
    
    protected static Optional<(Easing, int, int)> ParseDefaultParameters(string[] parameters)
    {
        const int maxEasingValue = 34;
        if (!int.TryParse(parameters[1], CultureInfo.InvariantCulture, out int easingInt))
        {
            return new OptionalError(OptionalErrorType.InvalidValue, $"Failed to parse Easing value: {parameters[1]}");
        }

        if (easingInt > maxEasingValue)
        {
            return new OptionalError(OptionalErrorType.InvalidValue, $"Invalid Easing value: {parameters[1]}"); 
        }

        var easing = (Easing)easingInt;
        
        if (!int.TryParse(parameters[2], CultureInfo.InvariantCulture, out int startTime))
        {
            return new OptionalError(OptionalErrorType.InvalidValue, $"Failed to parse start time value: {parameters[2]}");
        }
        
        if (string.IsNullOrWhiteSpace(parameters[3]))
        {
            return (easing, startTime, startTime);
        }
        
        if (!int.TryParse(parameters[3], CultureInfo.InvariantCulture, out int endTime))
        {
            return new OptionalError(OptionalErrorType.InvalidValue, $"Failed to parse end time value: {parameters[3]}");
        }

        return (easing, startTime, endTime);
    }
}