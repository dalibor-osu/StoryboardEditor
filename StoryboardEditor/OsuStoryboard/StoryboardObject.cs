using System.Globalization;
using System.Numerics;
using StoryboardEditor.OsuStoryboard.Commands;
using StoryboardEditor.OsuStoryboard.Enums;
using StoryboardEditor.Types.OptionalValue;

namespace StoryboardEditor.OsuStoryboard;

public abstract class StoryboardObject
{
    public Layer Layer { get; init; }
    public Origin Origin { get; init; }
    public string FilePath { get; init; }
    public Vector2 Position { get; init; }
    public StoryboardCommand[] Commands { get; private set; } = Array.Empty<StoryboardCommand>();

    public bool SetCommands(StoryboardCommand[] commands)
    {
        if (Commands.Length != 0)
        {
            return false;
        }

        Commands = commands;
        return true;
    }
    
    public static Optional<StoryboardObject> FromStoryboardLine(string line)
    {
        string[] properties = line.Split(',');

        return properties[0] switch
        {
            "Sprite" => SpriteFromStoryboardLine(properties),
            "Animation" => AnimationFromStoryboardLine(properties),
            _ => new OptionalError(OptionalErrorType.InvalidValue,
                $"Invalid storyboard object type: {properties[0]}")
        };
    }

    private static Optional<StoryboardObject> SpriteFromStoryboardLine(string[] properties)
    {
        const int spritePropertiesLength = 6;
        if (properties.Length != spritePropertiesLength)
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Storyboard sprite initialization must contain {spritePropertiesLength} values");
        }

        var optionalLayerOriginPath = ParseLayerOriginPathAndPosition(properties);
        if (!optionalLayerOriginPath.HasValue)
        {
            return new Optional<StoryboardObject>(optionalLayerOriginPath.Error);
        }

        (var layer, var origin, string filePath, var position) = optionalLayerOriginPath.Value;

        return new StoryboardSprite
        {
            FilePath = filePath,
            Layer = layer,
            Origin = origin,
            Position = position
        };
    }
    
    private static Optional<StoryboardObject> AnimationFromStoryboardLine(string[] properties)
    {
        const int animationPropertiesLength = 9;
        if (properties.Length != animationPropertiesLength)
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Storyboard animation initialization must contain {animationPropertiesLength} values");
        }

        var optionalLayerOriginPath = ParseLayerOriginPathAndPosition(properties);
        if (!optionalLayerOriginPath.HasValue)
        {
            return optionalLayerOriginPath.Error;
        }

        (var layer, var origin, string filePath, var position) = optionalLayerOriginPath.Value;

        if (!int.TryParse(properties[6], CultureInfo.InvariantCulture, out int frameCount))
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Invalid frame count in storyboard animation declaration: {properties[6]}");
        }

        if (frameCount <= 0)
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Invalid frame count in storyboard animation declaration: {properties[6]}. Value must be greater than 0.");
        }
        
        if (!int.TryParse(properties[6], CultureInfo.InvariantCulture, out int frameDelay))
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Invalid frame delay in storyboard animation declaration: {properties[6]}");
        }

        if (frameDelay <= 0)
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Invalid frame delay in storyboard animation declaration: {properties[6]}. Value must be greater than 0.");
        }
        
        if (!Enum.TryParse(properties[7], out LoopType loopType))
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Invalid loop type in storyboard animation declaration: {properties[7]}");
        }

        return new StoryboardAnimation
        {
            FilePath = filePath,
            Layer = layer,
            Origin = origin,
            Position = position,
            FrameCount = frameCount,
            FrameDelay = frameDelay,
            LoopType = loopType
        };
    }

    private static Optional<(Layer, Origin, string, Vector2)> ParseLayerOriginPathAndPosition(string[] properties)
    {
        if (!Enum.TryParse(properties[1], out Layer layer))
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Invalid layer in storyboard object declaration: {properties[1]}");
        }
        
        if (!Enum.TryParse(properties[2], out Origin origin))
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Invalid origin in storyboard object declaration: {properties[2]}");
        }
        
        if (!float.TryParse(properties[4], CultureInfo.InvariantCulture, out float x))
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Invalid x value in storyboard object declaration: {properties[4]}");
        }
        
        if (!float.TryParse(properties[5], CultureInfo.InvariantCulture, out float y))
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                $"Invalid x value in storyboard object declaration: {properties[5]}");
        }
        
        return (layer, origin, properties[3], new Vector2(x, y));
    }
}