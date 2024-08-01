namespace StoryboardEditor.Sound;

public abstract class EditorSound(string filePath)
{
    public virtual bool IsPlaying { get; protected set; }
    public string FilePath { get; init; } = filePath;
    public float Length { get; protected init; }

    private float _volume = 1.0f;

    public float Volume
    {
        get => _volume;
        set
        {
            _volume = value switch
            {
                > 1.0f => 1.0f,
                < 0.0f => 0.0f,
                _ => value
            };
            VolumeChanged();
        }
    }

    public abstract void Pause();
    public abstract void Stop();
    public abstract void Play();
    public abstract void Play(float volume);
    protected virtual void VolumeChanged() { }
    public void PausePlay()
    {
        if (IsPlaying)
        {
            Pause();
        }
        else
        {
            Play();
        }
    }
    public abstract void Unload();
    
    public void Restart()
    {
        Stop();
        Play();
    }
}