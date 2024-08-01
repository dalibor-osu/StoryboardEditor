using PrettyLogSharp;
using Raylib_cs;

namespace StoryboardEditor.Sound;

public class EditorSoundEffect : EditorSound
{
    public override bool IsPlaying => Raylib.IsSoundPlaying(_soundStream);

    private readonly Raylib_cs.Sound _soundStream;
    
    public EditorSoundEffect(string filePath) : base(filePath)
    {
        _soundStream = Raylib.LoadSound(filePath);
        Length = (float)_soundStream.FrameCount / _soundStream.Stream.SampleRate;
    }
    
    public override void Pause()
    {
        Raylib.PauseSound(_soundStream);
    }

    public override void Stop()
    {
        Raylib.StopSound(_soundStream);
    }

    public override void Play()
    {
        if (Raylib.IsSoundReady(_soundStream))
        {
            PrettyLogger.Log("Couldn't play sound");
            return;
        }
        Raylib.PlaySound(_soundStream);
    }

    public override void Play(float volume)
    {
        if (Raylib.IsSoundReady(_soundStream))
        {
            PrettyLogger.Log("Couldn't play sound");
            return;
        }
        
        float originalVolume = Volume;
        Volume = volume;
        Play();
        Volume = originalVolume;
    }

    public override void Unload()
    {
        Stop();
        Raylib.UnloadSound(_soundStream);
    }
}