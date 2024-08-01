using PrettyLogSharp;
using Raylib_cs;

namespace StoryboardEditor.Sound;

public class EditorMusic : EditorSound
{
    public override bool IsPlaying => Raylib.IsMusicStreamPlaying(_musicStream);
    public float TimePlayed => Raylib.GetMusicTimePlayed(_musicStream);
    
    private readonly Music _musicStream;
    
    public EditorMusic(string filePath) : base(filePath)
    {
        _musicStream = Raylib.LoadMusicStream(filePath);
        Length = Raylib.GetMusicTimeLength(_musicStream);
    }
    
    public override void Pause()
    {
        Raylib.PauseMusicStream(_musicStream);
    }

    public override void Stop()
    {
        Raylib.StopMusicStream(_musicStream);
    }

    public override void Play()
    {
        if (Raylib.IsMusicStreamPlaying(_musicStream))
        {
            PrettyLogger.Log("Couldn't play music");
            return;
        }
        Raylib.PlayMusicStream(_musicStream);
    }

    public override void Play(float volume)
    {
        if (Raylib.IsMusicReady(_musicStream))
        {
            PrettyLogger.Log("Couldn't play music");
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
        Raylib.UnloadMusicStream(_musicStream);
    }

    public void Seek(float position)
    {
        if (position < 0.0f)
        {
            position = 0.0f;
        }
        
        if (position > Length)
        {
            position = 0.0f;
            Stop();
        }
        
        Raylib.SeekMusicStream(_musicStream, position);
    }

    public void Update()
    {
        if (!IsPlaying)
        {
            return;
        }
        Raylib.UpdateMusicStream(_musicStream);
    }

    protected override void VolumeChanged()
    {
        Raylib.SetMusicVolume(_musicStream, Volume);
    }
}