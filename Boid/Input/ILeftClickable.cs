using MonoGame.Extended;

namespace Boid.Input;

public interface ILeftClickable
{
    RectangleF LeftClickArea { get; }

    void LeftClickAction();
    void ChangeState(ClickState clickState);
}
