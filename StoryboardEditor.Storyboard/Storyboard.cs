using System.Text;
using StoryboardEditor.Utils.OptionalValue;

namespace StoryboardEditor.Storyboard;

public class Storyboard
{
    public StoryboardObject[] Objects { get; private set; } = Array.Empty<StoryboardObject>();

    public static Optional<Storyboard> FromOsb(string path)
    {
        if (!path.EndsWith(".osb"))
        {
            return new OptionalError(OptionalErrorType.InvalidValue,
                "Path must end with .osb");
        }

        if (!File.Exists(path))
        {
            return new OptionalError(OptionalErrorType.FileError,
                $"File at: {path} doesn't exist.");
        }

        int objectCount = 0;
        
        const int bufferSize = 128;
        using var fileStream = File.OpenRead(path);
        using var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, bufferSize);
        
        int lineIndex = -1;
        while (streamReader.ReadLine() is { } line)
        {
            lineIndex++;
            switch (line[0])
            {
                case 'S' or 'A':
                    objectCount++;
                    continue;
                case '_':
                case ' ':
                case '[':
                    continue;
                case '/':
                    if (line[1] == '/')
                    {
                        continue;
                    }
                        
                    return new OptionalError(OptionalErrorType.FileError,
                        $"Invalid .osb line {lineIndex + 1}");
                default:
                    return new OptionalError(OptionalErrorType.FileError,
                        $"Invalid .osb line {lineIndex + 1}");
            }
        }

        fileStream.Position = 0;
        streamReader.DiscardBufferedData();

        var objects = new StoryboardObject[objectCount];
        int objectIndex = 0;
        lineIndex = -1;
        
        while (streamReader.ReadLine() is { } line)
        {
            lineIndex++;
            switch (line[0])
            {
                case 'S' or 'A':
                    var storyboardObject = StoryboardObject.FromStoryboardLine(line);
                    if (!storyboardObject.HasValue)
                    {
                        return new OptionalError(storyboardObject.Error.ErrorType,
                            $"Error parsing an object on line {lineIndex + 1}", storyboardObject.Error);
                    }

                    objects[objectIndex] = storyboardObject.Value;
                    objectIndex++;
                    continue;
                default:
                    continue;
            }
        }

        return new Storyboard {Objects = objects};
    }
}