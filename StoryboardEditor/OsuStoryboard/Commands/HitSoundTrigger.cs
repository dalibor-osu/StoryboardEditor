using StoryboardEditor.OsuStoryboard.Enums.Trigger;

namespace StoryboardEditor.OsuStoryboard.Commands;

public sealed class HitSoundTrigger : Trigger
{
    public SampleSet SampleSet { get; init; }
    public SampleSet AdditionSampleSet { get; init; }
    public Addition Addition { get; init; }
    public int CustomSampleSet { get; init; }
    
    public HitSoundTrigger(
        int startTime,
        int endTime,
        StoryboardCommand[] childCommands,
        SampleSet sampleSet = SampleSet.All,
        SampleSet additionSampleSet = SampleSet.All,
        Addition addition = Addition.None,
        int customSampleSet = 0)
            : base(Enums.Trigger.TriggerType.HitSound, startTime, endTime, childCommands)
    {
        SampleSet = sampleSet;
        AdditionSampleSet = additionSampleSet;
        Addition = addition;
        CustomSampleSet = customSampleSet;
    }
}