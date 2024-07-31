using System.Text;
using Microsoft.Extensions.Logging;
using StoryboardEditor.OsuStoryboard.Commands;
using StoryboardEditor.Utils.OptionalValue;
using static PrettyLogSharp.PrettyLogger;

namespace StoryboardEditor.OsuStoryboard;

public sealed class Storyboard
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

        const int bufferSize = 512;
        using var fileStream = File.OpenRead(path);
        using var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, bufferSize);

        var objectCountResult = GetStoryboardObjectAndCommandMaxCountInOsb(streamReader);
        if (!objectCountResult.HasValue)
        {
            return new OptionalError("Error when parsing a storyboard.", objectCountResult.Error);
        }

        fileStream.Position = 0;
        streamReader.DiscardBufferedData();

        (int objectCount, int commandMaxCount) = objectCountResult.Value;
        var objects = new StoryboardObject[objectCount];
        int objectIndex = 0;
        int lineIndex = -1;
        int commandIndex = 0;

        StoryboardObject? currentStoryboardObject = null;
        var currentCommands = new StoryboardCommand[commandMaxCount];

        while (streamReader.ReadLine() is { } line)
        {
            lineIndex++;
            switch (line[0])
            {
                case 'S' or 'A':
                    if (currentStoryboardObject != null)
                    {
                        currentStoryboardObject.SetCommands(currentCommands[..commandIndex]);
                        Array.Clear(currentCommands);
                        objects[objectIndex] = currentStoryboardObject;
                        objectIndex++;
                    }

                    var storyboardObject = StoryboardObject.FromStoryboardLine(line);
                    if (!storyboardObject.HasValue)
                    {
                        return new OptionalError(storyboardObject.Error.ErrorType,
                            $"Error parsing an object on line {lineIndex + 1}", storyboardObject.Error);
                    }

                    currentStoryboardObject = storyboardObject.Value;
                    commandIndex = 0;
                    continue;
                case ' ' or '_':
                    if (currentStoryboardObject == null)
                    {
                        return new OptionalError(
                            OptionalErrorType.InvalidValue, "Failed to parse Storyboard. Tried to add command to null StoryboardObject.");
                    }

                    // TODO: Add Trigger and Loop support
                    if (line[1] == 'T' || line[1] == 'L')
                    {
                        Log($"Skipping Loop or Trigger command: {line[1]}", LogLevel.Debug);
                        continue;
                    }

                    var commandResult = StoryboardCommand.FromLine(line);
                    if (commandResult.HasError)
                    {
                        return new OptionalError($"Failed to parse Storyboard. Error on line: {lineIndex}", commandResult.Error);
                    }

                    currentCommands[commandIndex] = commandResult.Value;
                    commandIndex++;
                    continue;

                default:
                    continue;
            }
        }

        if (currentStoryboardObject != null)
        {
            currentStoryboardObject.SetCommands(currentCommands.ToArray());
            objects[objectIndex] = currentStoryboardObject;
        }

        return new Storyboard { Objects = objects };
    }

    private static Optional<(int, int)> GetStoryboardObjectAndCommandMaxCountInOsb(StreamReader reader)
    {
        int objectCount = 0;
        int lineIndex = -1;
        int maxCommands = 0;
        int currentCommands = 0;

        while (reader.ReadLine() is { } line)
        {
            lineIndex++;
            switch (line[0])
            {
                case 'S' or 'A':
                {
                    if (currentCommands > maxCommands)
                    {
                        maxCommands = currentCommands;
                    }

                    currentCommands = 0;
                    objectCount++;
                    continue;
                }
                case '_':
                case ' ':
                {
                    currentCommands++;
                    continue;
                }
                case '[':
                    continue;
                case '/':
                {
                    if (line[1] == '/')
                    {
                        continue;
                    }

                    return new OptionalError(OptionalErrorType.FileError, $"Invalid .osb line {lineIndex + 1}");
                }
                default:
                    return new OptionalError(OptionalErrorType.FileError, $"Invalid .osb line {lineIndex + 1}");
            }
        }

        return (objectCount, maxCommands);
    }
}