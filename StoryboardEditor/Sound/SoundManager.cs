using Microsoft.Extensions.Logging;
using PrettyLogSharp;
using StoryboardEditor.Types.OptionalValue;

namespace StoryboardEditor.Sound;

public class SoundManager
{
    private static readonly Lazy<SoundManager> LazyInstance = new(() => new SoundManager(), LazyThreadSafetyMode.PublicationOnly);
    public static SoundManager Instance => LazyInstance.Value;
    
    private readonly Dictionary<string, EditorMusic> _musicStreams = [];
    private readonly Dictionary<string, EditorSoundEffect> _soundStreams = [];

    public Optional<EditorMusic> AddMusic(string id, string path)
    {
        if (!File.Exists(path))
        {
            PrettyLogger.Log($"Couldn't find Music file at path: {path}", LogLevel.Warning);
            return new OptionalError(OptionalErrorType.Null, $"Couldn't find Music file at path: {path}");
        }

        var music = new EditorMusic(path);
        _musicStreams.Add(id, music);
        return music;
    }

    public Optional<EditorMusic> AddMusic(string path) => AddMusic(path, path);
    
    public Optional<EditorSoundEffect> AddSound(string id, string path)
    {
        if (!File.Exists(path))
        {
            PrettyLogger.Log($"Couldn't find Sound file at path: {path}", LogLevel.Warning);
            return new OptionalError(OptionalErrorType.Null, $"Couldn't find Sound file at path: {path}");
        }

        var sound = new EditorSoundEffect(path);
        _soundStreams.Add(id, sound);
        return sound;
    }

    public Optional<EditorSoundEffect> AddSound(string path) => AddSound(path, path);
    
    public Optional<EditorMusic> GetMusicById(string id)
    {
        if (_musicStreams.TryGetValue(id, out var music))
        {
            return music;
        }
        
        PrettyLogger.Log($"Couldn't find Music with id: {id}", LogLevel.Warning);
        return new OptionalError(OptionalErrorType.Null, $"Couldn't find music with id: {id}");

    }
    
    public void UpdateMusicStreams()
    {
        foreach (var musicStream in _musicStreams.Values)
        {
            musicStream.Update();
        }
    }

    public void UnloadMusicStreams()
    {
        foreach (var musicStream in _musicStreams)
        {
            musicStream.Value.Unload();
            _musicStreams.Remove(musicStream.Key);
        }
    }
}