using System.Numerics;
using StoryboardEditor.OsuStoryboard.Enums;
using StoryboardEditor.Scenes;

namespace StoryboardEditor.WindowObjects;

public abstract class WindowObject
{
    public Scene ParentScene { get; init; }
    public Vector2 Position { get; init; }
    public Origin Origin { get; init; }
    public bool IsVisible { get; init; } = true;
    
    public virtual void OnSceneLoad() { }
    public virtual void Update() { }
    public virtual void Draw() { }
}