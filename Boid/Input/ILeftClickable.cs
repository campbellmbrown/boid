using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Boid.Input;

public interface ILeftClickable
{
    RectangleF LeftClickArea { get; }
    bool Focused { get; set;}

    void LeftClickAction();
    void ChangeState(ClickState clickState);
    void KeyPressed(Keys key);
}
