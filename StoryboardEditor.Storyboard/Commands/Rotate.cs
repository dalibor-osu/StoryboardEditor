using System.Globalization;
using StoryboardEditor.Utils.OptionalValue;

namespace StoryboardEditor.Storyboard.Commands;

public sealed class Rotate : StoryboardCommand
{
    public float[] Rotations { get; init; } = Array.Empty<float>();
    
    public static Optional<Rotate> FromLine(string line)
    {
        string[] split = line.Split(',');

        if (split.Length < 5)
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Failed to parse Rotate command line. Too few parameters: {line}");
        }
        
        var defaultParametersResult = ParseDefaultParameters(split);
        if (defaultParametersResult.HasError)
        {
            return new OptionalError("Error parsing a Rotate command", defaultParametersResult.Error);
        }

        (var easing, int startTime, int endTime) = defaultParametersResult.Value;

        int rotationsCount = split.Length - VALUES_OFFSET;
        float[] rotations = new float[rotationsCount];
        
        for (int i = 0; i < rotationsCount; i++)
        {
            int j = i + VALUES_OFFSET;
            if (!float.TryParse(split[j], CultureInfo.InvariantCulture, out float rotation))
            {
                return new OptionalError(OptionalErrorType.InvalidValue, $"Failed to parse rotation: {split[j]}");
            }

            rotations[i] = rotation;
        }

        return new Rotate()
        {
            Easing = easing,
            StartTime = startTime,
            EndTime = endTime,
            Rotations = rotations
        };
    }
}