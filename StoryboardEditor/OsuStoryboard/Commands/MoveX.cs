using System.Globalization;
using StoryboardEditor.Types.OptionalValue;

namespace StoryboardEditor.OsuStoryboard.Commands;

public sealed class MoveX : StoryboardCommand
{
    public float[] Positions { get; init; }
    
    public static Optional<MoveX> FromLine(string line)
    {
        string[] split = line.Split(',');

        if (split.Length < 5)
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Failed to parse MoveX command line. Too few parameters: {line}");
        }
        
        var defaultParametersResult = ParseDefaultParameters(split);
        if (defaultParametersResult.HasError)
        {
            return new OptionalError("Error parsing a MoveX command", defaultParametersResult.Error);
        }

        (var easing, int startTime, int endTime) = defaultParametersResult.Value;

        int positionsCount = split.Length - VALUES_OFFSET;
        float[] positions = new float[positionsCount];
        
        for (int i = 0; i < positionsCount; i++)
        {
            int j = i + VALUES_OFFSET;
            if (!float.TryParse(split[j], CultureInfo.InvariantCulture, out float position))
            {
                return new OptionalError(OptionalErrorType.InvalidValue, $"Failed to parse X position: {split[j]}");
            }

            positions[i] = position;
        }

        return new MoveX
        {
            Easing = easing,
            StartTime = startTime,
            EndTime = endTime,
            Positions = positions
        };
    }
}