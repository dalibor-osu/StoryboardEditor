using Microsoft.Extensions.Logging;
using PrettyLogSharp;
using StoryboardEditor.Enums;
using StoryboardEditor.Scenes;
using StoryboardEditor.Types.OptionalValue;

namespace StoryboardEditor;

public sealed class SceneManager
{
    private static readonly Lazy<SceneManager> LazyInstance = new(() => new SceneManager(), LazyThreadSafetyMode.PublicationOnly);
    public static SceneManager Instance => LazyInstance.Value;

    public Scene CurrentScene { get; private set; }

    private readonly Dictionary<string, Scene> _scenes = [];

    private SceneManager()
    {
    }

    public void AddScene<T>(string id) where T : Scene
    {
        AddSceneWithParams<T>(id);
    }

    public T AddSceneWithParams<T>(string id, params object[] args) where T : Scene
    {
        object[] parameters = args.Prepend(id).ToArray();

        var instance = args.Length > 0 ? (T?)Activator.CreateInstance(typeof(T), parameters) : (T?)Activator.CreateInstance(typeof(T), id);
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
        CurrentScene = instance;
    }

    public bool RemoveScene(string id)
    {
        if (CurrentScene.Id != id)
        {
            return _scenes.Remove(id);
        }

        PrettyLogger.Log($"Can't remove activated scene \"{id}\"", LogLevel.Warning);
        return false;
    }

    public Optional<Scene> LoadAndActivateScene(string id)
    {
        if (_scenes.TryGetValue(id, out var scene))
        {
            if (scene.LoadingStatus == SceneLoadingStatus.NotLoaded)
            {
                scene.OnLoad();
            }

            if (scene.LoadingStatus == SceneLoadingStatus.Loading)
            {
                Task.WaitAll();
            }

            scene.OnActivate();
            CurrentScene = scene;
            return scene;
        }

        PrettyLogger.Log($"Scene \"{id}\" does not exist", LogLevel.Error);
        return new OptionalError(OptionalErrorType.Null, $"Scene {id} doesn't exist.");
    }

    public Optional<Scene> LoadScene(string id)
    {
        if (_scenes.TryGetValue(id, out var scene))
        {
            if (scene.LoadingStatus is SceneLoadingStatus.Loaded or SceneLoadingStatus.Loading)
            {
                PrettyLogger.Log($"Scene {id} is already loading or loaded.", LogLevel.Warning);
                return scene;
            }

            scene.OnLoad();
            return scene;
        }

        PrettyLogger.Log($"Scene \"{id}\" does not exist", LogLevel.Error);
        return new OptionalError(OptionalErrorType.Null, $"Scene {id} doesn't exist.");
    }

    public Optional<Scene> ActivateScene(string id)
    {
        if (_scenes.TryGetValue(id, out var scene))
        {
            if (scene.Id == CurrentScene.Id)
            {
                PrettyLogger.Log($"Scene {id} is already activated.", LogLevel.Warning);
                return scene;
            }

            scene.OnActivate();
            CurrentScene = scene;
            return scene;
        }

        PrettyLogger.Log($"Scene \"{id}\" does not exist", LogLevel.Error);
        return new OptionalError(OptionalErrorType.Null, $"Scene {id} doesn't exist.");
    }

    public Optional<Scene> StartLoadingScene(string id)
    {
        if (_scenes.TryGetValue(id, out var scene))
        {
            if (scene.LoadingStatus is SceneLoadingStatus.Loaded or SceneLoadingStatus.Loading)
            {
                PrettyLogger.Log($"Scene {id} is already loading or loaded.", LogLevel.Warning);
                return scene;
            }

            Task.Run(scene.OnLoad);
            return scene;
        }

        PrettyLogger.Log($"Scene \"{id}\" does not exist", LogLevel.Error);
        return new OptionalError(OptionalErrorType.Null, $"Scene {id} doesn't exist.");
    }

    public Optional<Scene> StartLoadingSceneWithCallback(string id, Action<Scene> callback)
    {
        if (_scenes.TryGetValue(id, out var scene))
        {
            if (scene.LoadingStatus is SceneLoadingStatus.Loaded or SceneLoadingStatus.Loading)
            {
                PrettyLogger.Log($"Scene {id} is already loading or loaded.", LogLevel.Warning);
                return scene;
            }

            Task.Run(() =>
            {
                scene.OnLoad();
                callback(scene);
            });

            return scene;
        }

        PrettyLogger.Log($"Scene \"{id}\" does not exist", LogLevel.Error);
        return new OptionalError(OptionalErrorType.Null, $"Scene {id} doesn't exist.");
    }
}