using StoryboardEditor.Storyboard.Enums.Trigger;

namespace StoryboardEditor.Storyboard.Commands;

public class Trigger
{
    public TriggerType TriggerType { get; init; }
    public int StartTime { get; init; }
    public int EndTime { get; init; }
    public StoryboardCommand[] ChildCommands { get; init; }

    public Trigger(TriggerType triggerType, int startTime, int endTime, StoryboardCommand[] childCommands)
    {
        TriggerType = triggerType;
        StartTime = startTime;
        EndTime = endTime;
        ChildCommands = childCommands;
    }
}