using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PrettyLogSharp;
using StoryboardEditor.Storyboard;
using static PrettyLogSharp.PrettyLogger;

const string storyboardFilePath = "Assets/dj_mag.osb";

Log("Parsing storyboard file at " + storyboardFilePath, LogLevel.Debug);

var stopwatch = Stopwatch.StartNew();
var storyboard = Storyboard.FromOsb(storyboardFilePath);
stopwatch.Stop();

if (storyboard.HasValue)
{
    Log($"Storyboard was successfully parsed with {storyboard.Value.Objects.Length} objects!", LogType.Success);
    Log($"Parsing took {stopwatch.ElapsedMilliseconds} milliseconds.", LogType.Success);

    var maxCommandsLength = storyboard.Value.Objects.MaxBy(x => x.Commands.Length);
    Log($"Max amount of commands on a single object was {maxCommandsLength}", LogLevel.Debug);
}
else
{
    Log("Failed to parse storyboard.", LogType.Exception);
    Log(storyboard.Error.ToString(), LogType.Exception);
}