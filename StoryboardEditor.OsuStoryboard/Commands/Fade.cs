using System.Globalization;
using StoryboardEditor.Utils.OptionalValue;

namespace StoryboardEditor.OsuStoryboard.Commands;

public sealed class Fade : StoryboardCommand
{
    public float[] Opacities { get; init; } = Array.Empty<float>();

    public static Optional<Fade> FromLine(string line)
    {
        string[] split = line.Split(',');

        if (split.Length < 5)
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Failed to parse Fade command line. Too few parameters: {line}");
        }
        
        var defaultParametersResult = ParseDefaultParameters(split);
        if (defaultParametersResult.HasError)
        {
            return new OptionalError("Error parsing a Fade command", defaultParametersResult.Error);
        }

        (var easing, int startTime, int endTime) = defaultParametersResult.Value;

        int opacitiesCount = split.Length - VALUES_OFFSET;
        float[] opacities = new float[opacitiesCount];
        
        for (int i = 0; i < opacitiesCount; i++)
        {
            int j = i + VALUES_OFFSET;
            if (!float.TryParse(split[j], CultureInfo.InvariantCulture, out float opacity))
            {
                return new OptionalError(OptionalErrorType.InvalidValue, $"Failed to parse opacity: {split[j]}");
            }

            opacities[i] = opacity;
        }

        return new Fade
        {
            Easing = easing,
            StartTime = startTime,
            EndTime = endTime,
            Opacities = opacities
        };
    }
}