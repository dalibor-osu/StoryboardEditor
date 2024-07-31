using PrettyLogSharp;
using Raylib_cs;

namespace StoryboardEditor.Scenes;

public sealed class MainMenu : Scene
{
    public override void Draw(float deltaTime)
    {
        Raylib.DrawText("Main menu!", 12, 12, 20, Color.Black);
    }

    public override void HandleInput(float deltaTime)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            PrettyLogger.Log("Changing scene to player");
            SceneManager.Instance.LoadScene("player");
        }
    }
}