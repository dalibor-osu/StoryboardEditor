using System.Numerics;
using Raylib_cs;
using StoryboardEditor.Scenes;
using static PrettyLogSharp.PrettyLogger;

namespace StoryboardEditor;

public class Game
{
    private const int DefaultScreenWidth = 1280;
    private const int DefaultScreenHeight = 720;
    private const int TargetFps = 120;
    private const string GameTitle = "Storyboard Editor";
    
    public static int WindowWidth { get; private set; }
    public static int WindowHeight { get; private set; }
    
    private readonly SceneManager _sceneManager = SceneManager.Instance;
    
    private static bool _exit;

    public void StartGame()
    {
        Initialize();
        RunMainGameLoop();
        Close();
    }

    private void RunMainGameLoop()
    {
        while (!Raylib.WindowShouldClose() && !_exit)
        {
            Raylib.BeginDrawing();

                Raylib.ClearBackground(Color.RayWhite);

                var currentScene = _sceneManager.CurrentScene;
                float deltaTime = Raylib.GetFrameTime();
                currentScene.HandleInput(deltaTime);
                currentScene.Draw(deltaTime);
                
            Raylib.EndDrawing();
        }
    }
    
    private void Initialize()
    {
#if DEBUG
        Raylib.SetTraceLogLevel(TraceLogLevel.Warning);
#else
        Raylib.SetTraceLogLevel(TraceLogLevel.Error);
#endif
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(DefaultScreenWidth, DefaultScreenHeight, GameTitle);
        Raylib.SetExitKey(KeyboardKey.Insert);
        Raylib.InitAudioDevice();
        Raylib.SetTargetFPS(TargetFps);
        
        int currentMonitor = Raylib.GetCurrentMonitor();
        
        int monitorHeight = Raylib.GetMonitorHeight(currentMonitor);
        int monitorWidth = Raylib.GetMonitorWidth(currentMonitor);
        
        WindowHeight = (int)(Raylib.GetMonitorHeight(currentMonitor) * 0.8f);
        WindowWidth = (int)(Raylib.GetMonitorWidth(currentMonitor) * 0.8f);
        
        Log($"Window size: {WindowWidth}x{WindowHeight}");

        Raylib.SetWindowSize(WindowWidth, WindowHeight);
        Raylib.SetWindowPosition(monitorWidth / 2 - WindowWidth / 2, monitorHeight / 2 - WindowHeight / 2);
        
        _sceneManager.AddScene<MainMenu>("mainMenu");
        _sceneManager.AddSceneWithParams<Player>("player", "Assets/dj_mag.osb");
        _sceneManager.LoadScene("mainMenu");
    }
    
    private void Close()
    {
        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }
    
    public static Vector2 GetWindowCentre()
    {
        return new Vector2(WindowWidth / 2f, WindowHeight / 2f);
    }

    public static void Exit()
    {
        _exit = true;
    }
}