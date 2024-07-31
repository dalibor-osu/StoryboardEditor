using System.Globalization;
using System.Numerics;
using StoryboardEditor.Utils.OptionalValue;
using StoryboardEditor.Utils.Types;

namespace StoryboardEditor.OsuStoryboard.Commands;

public sealed class Move : StoryboardCommand
{
    public Vector2 StartPosition { get; init; }
    public Vector2 EndPosition { get; init; }
    
    public static Optional<Move> FromLine(string line)
    {
        string[] split = line.Split(',');

        if (split.Length < 6)
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Failed to parse Move command line. Too few parameters: {line}");
        }
        
        var defaultParametersResult = ParseDefaultParameters(split);
        if (defaultParametersResult.HasError)
        {
            return new OptionalError("Error parsing a Move command", defaultParametersResult.Error);
        }

        (var easing, int startTime, int endTime) = defaultParametersResult.Value;
        Vector2 startPosition = new(), endPosition = new();
        const int vectorLength = 2;
        
        for (int i = 0; i < vectorLength; i++)
        {
            if (!float.TryParse(split[VALUES_OFFSET + i], CultureInfo.InvariantCulture, out float positionValue))
            {
                return new OptionalError(OptionalErrorType.InvalidValue,
                    $"Failed to parse Position value: {split[VALUES_OFFSET + i]}");
            }

            startPosition[i] = positionValue;
        }

        if (split.Length < 8)
        {
            endPosition = startPosition;
        }
        else
        {
            for (int i = 0; i < vectorLength; i++)
            {
                if (!float.TryParse(split[VALUES_OFFSET + vectorLength + i], CultureInfo.InvariantCulture, out float positionValue))
                {
                    return new OptionalError(OptionalErrorType.InvalidValue,
                        $"Failed to parse Position value: {split[VALUES_OFFSET + i]}");
                }

                endPosition[i] = positionValue;
            }
        }
        
        return new Move
        {
            Easing = easing,
            StartTime = startTime,
            EndTime = endTime,
            StartPosition = startPosition,
            EndPosition = endPosition
        };
    }
}