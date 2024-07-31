using StoryboardEditor.OsuStoryboard.Enums.Trigger;

namespace StoryboardEditor.OsuStoryboard.Commands;

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