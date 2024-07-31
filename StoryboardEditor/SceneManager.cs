using Microsoft.Extensions.Logging;
using PrettyLogSharp;
using StoryboardEditor.Scenes;

namespace StoryboardEditor;

public sealed class SceneManager
{
    private static readonly Lazy<SceneManager> LazyInstance = new(() => new SceneManager(), LazyThreadSafetyMode.PublicationOnly);
    public static SceneManager Instance => LazyInstance.Value;

    private Scene _currentScene;
    public Scene CurrentScene => _currentScene;
    private readonly Dictionary<string, Scene> _scenes = [];
    
    private SceneManager() { }
    
    public void AddScene<T>(string id) where T : Scene, new()
    {
        _scenes.Add(id, new T());
    }

    public T AddSceneWithParams<T>(string id, params object[] args) where T : Scene
    {
        var instance = (T?)Activator.CreateInstance(typeof(T), args);
        if (instance == null)
        {
            throw new Exception("Failed to initialize Scene instance");
        }
        _scenes.Add(id, instance);
        return instance;
    }

    public void AddAndLoadSceneWithParams<T>(string id, params object[] args) where T : Scene, new()
    {
        var instance = AddSceneWithParams<T>(id, args);
        _currentScene = instance;
    }
    
    public void RemoveScene(string id)
    {
        _scenes.Remove(id);
    }

    public void LoadScene(string id)
    {
        if (_scenes.TryGetValue(id, out var scene))
        {
            _currentScene = scene;
            _currentScene.OnLoad();
        }
        else
        {
            PrettyLogger.Log($"Scene \"{id}\" does not exist", LogLevel.Error);
        }
    }
}