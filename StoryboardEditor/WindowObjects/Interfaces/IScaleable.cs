using System.Numerics;

namespace StoryboardEditor.WindowObjects.Interfaces;

public interface IScalable
{
    private const float VectorScaleTolerance = 10 ^ -3;
    public Vector2 Scale { get; set; }
    public bool IsVectorScaled => Math.Abs(Scale.X - Scale.Y) > VectorScaleTolerance;
}