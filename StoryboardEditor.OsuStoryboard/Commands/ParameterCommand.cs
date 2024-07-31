using StoryboardEditor.OsuStoryboard.Enums;
using StoryboardEditor.Utils.OptionalValue;

namespace StoryboardEditor.OsuStoryboard.Commands;

public class ParameterCommand : StoryboardCommand
{
    public ParameterCommandType Type { get; init; }
    
    public static Optional<ParameterCommand> FromLine(string line)
    {
        string[] split = line.Split(',');

        if (split.Length < 5)
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Failed to parse Parameter command line. Too few parameters: {line}");
        }
        
        var defaultParametersResult = ParseDefaultParameters(split);
        if (defaultParametersResult.HasError)
        {
            return new OptionalError("Error parsing a Parameter command", defaultParametersResult.Error);
        }

        (var easing, int startTime, int endTime) = defaultParametersResult.Value;

        ParameterCommandType type;
        
        switch (split[4][0])
        {
            case 'H':
                type = ParameterCommandType.FlipHorizontally;
                break;
            case 'V':
                type = ParameterCommandType.FlipVertically;
                break;
            case 'A':
                type = ParameterCommandType.UseAdditive;
                break;
            default:
                return new OptionalError(OptionalErrorType.InvalidValue,
                    $"Failed to parse Parameter command type: {split[4][0]}");
        }

        return new ParameterCommand
        {
            Easing = easing,
            StartTime = startTime,
            EndTime = endTime,
            Type = type
        };
    }
}