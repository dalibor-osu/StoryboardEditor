namespace StoryboardEditor.WindowObjects.Interfaces;

public interface IClickable
{
    public void OnClick() {}
    public virtual void OnPointerEnter() {}
    public virtual void OnPointerStay() {}
    public virtual void OnPointerExit() {}
}