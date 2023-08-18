using Microsoft.Xna.Framework.Input;

namespace Boid.Input;

public interface IMouseWrapper
{
    public MouseState MouseState { get; }
}

public class MouseWrapper : IMouseWrapper
{
    public MouseState MouseState => Mouse.GetState();
}
