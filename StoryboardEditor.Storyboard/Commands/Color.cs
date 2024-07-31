using System.Globalization;
using StoryboardEditor.Utils.OptionalValue;
using StoryboardEditor.Utils.Types;

namespace StoryboardEditor.Storyboard.Commands;

public sealed class Color : ParameterCommand
{
    public Color3 StartColor { get; init; }
    public Color3 EndColor { get; init; }
    
    public static Optional<Color> FromLine(string line)
    {
        string[] split = line.Split(',');

        if (split.Length < 7)
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Failed to parse Color command line. Too few parameters: {line}");
        }
        
        var defaultParametersResult = ParseDefaultParameters(split);
        if (defaultParametersResult.HasError)
        {
            return new OptionalError("Error parsing a Color command", defaultParametersResult.Error);
        }

        (var easing, int startTime, int endTime) = defaultParametersResult.Value;
        Color3 startColor = new(), endColor = new();
        const int colorVectorLength = 3;
        
        for (int i = 0; i < colorVectorLength; i++)
        {
            if (!ushort.TryParse(split[VALUES_OFFSET + i], out ushort colorValue))
            {
                return new OptionalError(OptionalErrorType.InvalidValue,
                    $"Failed to parse Color value: {split[VALUES_OFFSET + i]}");
            }

            startColor[i] = colorValue;
        }

        if (split.Length < 10)
        {
            endColor = startColor;
        }
        else
        {
            for (int i = 0; i < colorVectorLength; i++)
            {
                if (!ushort.TryParse(split[VALUES_OFFSET + colorVectorLength + i], out ushort colorValue))
                {
                    return new OptionalError(OptionalErrorType.InvalidValue,
                        $"Failed to parse Color value: {split[VALUES_OFFSET + i]}");
                }

                endColor[i] = colorValue;
            }
        }

        return new Color
        {
            Easing = easing,
            StartTime = startTime,
            EndTime = endTime,
            StartColor = startColor,
            EndColor = endColor
        };
    }
}