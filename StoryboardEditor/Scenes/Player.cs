using System.Numerics;
using Microsoft.Extensions.Logging;
using PrettyLogSharp;
using Raylib_cs;
using StoryboardEditor.Enums;
using StoryboardEditor.OsuStoryboard;
using StoryboardEditor.Sound;
using StoryboardEditor.WindowObjects;
using StoryboardEditor.WindowObjects.Interfaces;

namespace StoryboardEditor.Scenes;

public class Player : Scene
{
    private readonly string _osbPath;
    private Storyboard _storyboard = new();
    private EditorMusic? _editorMusic;
    
    public Player(string id, string osbPath) : base(id)
    {
        PrettyLogger.Log("Creating Player instance");
        _osbPath = osbPath;
    }

    public override void OnLoad()
    {
        LoadingStatus = SceneLoadingStatus.Loading;
        
        var storyboardResult = Storyboard.FromOsb(_osbPath);
        if (storyboardResult.HasError)
        {
            PrettyLogger.Log("Failed to load Player scene.", LogLevel.Error);
            PrettyLogger.Log(storyboardResult.Error.ToString(), LogLevel.Error);
            SceneManager.Instance.LoadAndActivateScene("mainMenu");
            return;
        }

        _storyboard = storyboardResult.Value;

        var musicResult = SoundManager.Instance.AddMusic("Assets/3_minutes.mp3");
        if (musicResult.HasValue)
        {
            _editorMusic = musicResult.Value;
        }
        
        ChildObjects.AddRange([
        new CustomWindowObject
        {
            Position = new Vector2(12, 12),
            CustomDraw = self =>
            {
                Raylib.DrawText($"Loaded {_storyboard.Objects.Length} storyboard objects.", (int)self.Position.X, (int)self.Position.Y, 20, Color.Black);
            }
        },
        new CustomWindowObject
        {
            Position = new Vector2(12, 12 * 4),
            CustomDraw = self =>
            {
                if (_editorMusic != null)
                {
                    Raylib.DrawText($"Play time: {_editorMusic.TimePlayed}", (int)self.Position.X, (int)self.Position.Y, 20, Color.Black);
                }
            }
        },
        new SongPositionSlider
        {
            ParentScene = this,
            Position = new Vector2(100, 100),
            Size = new Vector2(500, 20),
            Music = _editorMusic
        }
        ]);
        
        PrettyLogger.Log("Load task finished");
        LoadingStatus = SceneLoadingStatus.Loaded;
    }

    public override void HandleInput(float deltaTime)
    {
        if (_editorMusic == null)
        {
            return;
        }
        
        if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            _editorMusic.PausePlay();
        }
        
        if (Raylib.IsKeyPressed(KeyboardKey.S))
        {
            _editorMusic.Stop();
        }
        
        if (Raylib.IsKeyPressed(KeyboardKey.R))
        {
            _editorMusic.Restart();
        }
        
        if (Raylib.IsKeyPressed(KeyboardKey.Down))
        {
            _editorMusic.Volume -= 0.1f;
        }
        
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
        {
            _editorMusic.Volume += 0.1f;
        }

        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            foreach (var obj in ChildObjects.Where(x => x is IClickable))
            {
                ((IClickable)obj).OnClick();
            }
        }
    }
}