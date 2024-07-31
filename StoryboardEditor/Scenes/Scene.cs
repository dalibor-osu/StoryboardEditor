using Raylib_cs;

namespace StoryboardEditor.Scenes;

public abstract class Scene
{
    public abstract void Draw(float deltaTime);

    public virtual void OnLoad()
    {
        
    }

    public virtual void HandleInput(float deltaTime)
    {
        
    }
}