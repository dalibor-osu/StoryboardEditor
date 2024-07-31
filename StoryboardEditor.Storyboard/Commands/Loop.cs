namespace StoryboardEditor.Storyboard.Commands;

public sealed class Loop
{
    public int StartTime { get; init; }
    public int LoopCount { get; init; }
    public StoryboardCommand[] ChildCommands { get; init; }
}