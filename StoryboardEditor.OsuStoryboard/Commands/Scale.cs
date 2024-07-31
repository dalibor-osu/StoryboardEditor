using System.Globalization;
using StoryboardEditor.Utils.OptionalValue;

namespace StoryboardEditor.OsuStoryboard.Commands;

public sealed class Scale : StoryboardCommand
{
    public float[] Scales { get; init; } = Array.Empty<float>();
    
    public static Optional<Scale> FromLine(string line)
    {
        string[] split = line.Split(',');

        if (split.Length < 5)
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Failed to parse Scale command line. Too few parameters: {line}");
        }
        
        var defaultParametersResult = ParseDefaultParameters(split);
        if (defaultParametersResult.HasError)
        {
            return new OptionalError("Error parsing a Scale command", defaultParametersResult.Error);
        }

        (var easing, int startTime, int endTime) = defaultParametersResult.Value;

        int scalesCount = split.Length - VALUES_OFFSET;
        float[] scales = new float[scalesCount];
        
        for (int i = 0; i < scalesCount; i++)
        {
            int j = i + VALUES_OFFSET;
            if (!float.TryParse(split[j], CultureInfo.InvariantCulture, out float scale))
            {
                return new OptionalError(OptionalErrorType.InvalidValue, $"Failed to parse scale: {split[j]}");
            }

            scales[i] = scale;
        }

        return new Scale
        {
            Easing = easing,
            StartTime = startTime,
            EndTime = endTime,
            Scales = scales
        };
    }
}