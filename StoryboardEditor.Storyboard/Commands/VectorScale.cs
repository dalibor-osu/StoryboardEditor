using System.Globalization;
using System.Numerics;
using StoryboardEditor.Utils.OptionalValue;

namespace StoryboardEditor.Storyboard.Commands;

public sealed class VectorScale : StoryboardCommand
{
    public Vector2 StartScale { get; init; }
    public Vector2 EndScale { get; init; }
    
    public static Optional<VectorScale> FromLine(string line)
    {
        string[] split = line.Split(',');

        if (split.Length < 6)
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Failed to parse VectorScale command. Too few parameters: {line}");
        }
        
        var defaultParametersResult = ParseDefaultParameters(split);
        if (defaultParametersResult.HasError)
        {
            return new OptionalError("Error parsing a VectorScale command", defaultParametersResult.Error);
        }

        (var easing, int startTime, int endTime) = defaultParametersResult.Value;
        Vector2 startScale = new(), endScale = new();
        const int vectorLength = 2;
        
        for (int i = 0; i < vectorLength; i++)
        {
            if (!float.TryParse(split[VALUES_OFFSET + i], CultureInfo.InvariantCulture, out float scaleValue))
            {
                return new OptionalError(OptionalErrorType.InvalidValue,
                    $"Failed to parse VectorScale value: {split[VALUES_OFFSET + i]}");
            }

            startScale[i] = scaleValue;
        }

        if (split.Length < 8)
        {
            endScale = startScale;
        }
        else
        {
            for (int i = 0; i < vectorLength; i++)
            {
                if (!float.TryParse(split[VALUES_OFFSET + vectorLength + i], CultureInfo.InvariantCulture, out float scaleValue))
                {
                    return new OptionalError(OptionalErrorType.InvalidValue,
                        $"Failed to parse VectorScale value: {split[VALUES_OFFSET + i]}");
                }

                endScale[i] = scaleValue;
            }
        }
        
        return new VectorScale
        {
            Easing = easing,
            StartTime = startTime,
            EndTime = endTime,
            StartScale = startScale,
            EndScale = endScale
        };
    }
}