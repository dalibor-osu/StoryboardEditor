using Microsoft.Extensions.Logging;
using PrettyLogSharp;
using Raylib_cs;
using StoryboardEditor.OsuStoryboard;

namespace StoryboardEditor.Scenes;

public class Player : Scene
{
    private readonly string _osbPath;
    private Storyboard _storyboard = new();
    private readonly Task _loadTask;
    private int _textX = 12;
    
    public Player(string osbPath)
    {
        PrettyLogger.Log("Creating Player instance");
        _osbPath = osbPath;
        _loadTask = new Task(() =>
        {
            var storyboardResult = Storyboard.FromOsb(_osbPath);
            if (storyboardResult.HasError)
            {
                PrettyLogger.Log("Failed to load Player scene.", LogLevel.Error);
                PrettyLogger.Log(storyboardResult.Error.ToString(), LogLevel.Error);
                SceneManager.Instance.LoadScene("mainMenu");
                return;
            }

            _storyboard = storyboardResult.Value;
            PrettyLogger.Log("Load task finished");
        });
    }
    
    public override void OnLoad()
    {
        PrettyLogger.Log("Starting load task");
        _loadTask.Start();
    }

    public override void Draw(float deltaTime)
    {
        if (!_loadTask.IsCompleted)
        {
            Raylib.DrawText("Loading...", _textX, 12, 20, Color.Black);
            _textX += 1;
            return;
        }
        
        Raylib.DrawText($"Loaded {_storyboard.Objects.Length} storyboard objects.", 12, 12, 20, Color.DarkGreen);
    }
}