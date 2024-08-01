using System.Numerics;
using PrettyLogSharp;
using Raylib_cs;
using StoryboardEditor.Sound;
using StoryboardEditor.WindowObjects.Interfaces;

namespace StoryboardEditor.WindowObjects;

public class SongPositionSlider : WindowObject, IClickable
{
    public Vector2 Size { get; set; }
    public EditorMusic? Music { get; init; }
    
    public override void Draw()
    {
        Raylib.DrawRectangleV(Position, Size, Color.Black);
        if (Music == null)
        {
            return;
        }
        
        float played = Music.TimePlayed / Music.Length;
        var progressRecSize = Size with { X = Size.X * played };
        Raylib.DrawRectangleV(Position, progressRecSize, Color.Green);
    }

    public void OnClick()
    {
        if (Music == null)
        {
            return;
        }
        
        var mousePosition = Raylib.GetMousePosition();
        if (!Raylib.CheckCollisionPointRec(mousePosition, new Rectangle(Position, Size)))
        {
            return;
        }
        
        float posWithoutOffset = mousePosition.X - Position.X;
        float position = posWithoutOffset / Size.X * Music.Length;
        PrettyLogger.Log($"Mouse position X: {mousePosition.X}, Position X: {Position.X}, Size X: {Size.X}, Calculated position: {position}");
        Music.Seek(position);
    }
}