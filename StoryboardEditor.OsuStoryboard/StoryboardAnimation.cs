using StoryboardEditor.OsuStoryboard.Enums;

namespace StoryboardEditor.OsuStoryboard;

public class StoryboardAnimation : StoryboardObject
{
    public int FrameCount { get; set; }
    public int FrameDelay { get; set; }
    public LoopType LoopType { get; set; }
}