using Raylib_cs;
using StoryboardEditor.Enums;
using StoryboardEditor.Exceptions;
using StoryboardEditor.WindowObjects;

namespace StoryboardEditor.Scenes;

public abstract class Scene(string id)
{
    public SceneLoadingStatus LoadingStatus { get; protected set; } = SceneLoadingStatus.NotLoaded;
    public string Id { get; init; } = id;
    protected List<WindowObject> ChildObjects = [];

    public virtual void Draw(float deltaTime)
    {
        foreach (var child in ChildObjects)
        {
            child.Draw();
        }
    }

    public virtual void OnLoad()
    {
        LoadingStatus = SceneLoadingStatus.Loaded;
    }

    public virtual void OnActivate()
    {
        if (LoadingStatus != SceneLoadingStatus.Loaded)
        {
            throw new UnloadedCallException($"Tried to activate an unloaded scene {GetType()}!");
        }
    }

    public virtual void HandleInput(float deltaTime)
    {
        
    }
}