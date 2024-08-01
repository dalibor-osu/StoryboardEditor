namespace StoryboardEditor.WindowObjects;

public class CustomWindowObject : WindowObject
{
    public Action<CustomWindowObject> CustomDraw { get; init; }

    public override void Draw() => CustomDraw(this);
}