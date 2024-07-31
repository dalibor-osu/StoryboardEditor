using System.Globalization;
using StoryboardEditor.Utils.OptionalValue;

namespace StoryboardEditor.OsuStoryboard.Commands;

public sealed class MoveY : StoryboardCommand
{
    public float[] Positions { get; init; }
    
    public static Optional<MoveY> FromLine(string line)
    {
        string[] split = line.Split(',');

        if (split.Length < 5)
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Failed to parse MoveY command line. Too few parameters: {line}");
        }
        
        var defaultParametersResult = ParseDefaultParameters(split);
        if (defaultParametersResult.HasError)
        {
            return new OptionalError("Error parsing a MoveY command", defaultParametersResult.Error);
        }

        (var easing, int startTime, int endTime) = defaultParametersResult.Value;

        int positionsCount = split.Length - VALUES_OFFSET;
        float[] positions = new float[positionsCount];
        
        for (int i = 0; i < positionsCount; i++)
        {
            int j = i + VALUES_OFFSET;
            if (!float.TryParse(split[j], CultureInfo.InvariantCulture, out float position))
            {
                return new OptionalError(OptionalErrorType.InvalidValue, $"Failed to parse Y position: {split[j]}");
            }

            positions[i] = position;
        }

        return new MoveY
        {
            Easing = easing,
            StartTime = startTime,
            EndTime = endTime,
            Positions = positions
        };
    }
}