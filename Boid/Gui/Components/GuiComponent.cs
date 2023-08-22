using Boid.Visual;
using Microsoft.Xna.Framework;

namespace Boid.Gui.Components;

public interface IGuiComponent : IGuiElement, IVisualRelative, IFrameTickable
{
    void FinalizeComponent(int availableWidth, int availableHeight);
}

public abstract class GuiComponent : IGuiComponent
{
    protected Vector2 Position { get; set; } = Vector2.Zero;

    public int Width { get; protected init; }
    public int Height { get; protected init; }

    public virtual void FinalizeComponent(int availableWidth, int availableHeight)
    {
        // Do nothing by default.
    }

    public virtual void UpdatePosition(Vector2 position)
    {
        Position = position;
    }

    public abstract void Draw(ISpriteBatchWrapper spriteBatch);

    public virtual void FrameTick(IFrameTickManager frameTickManager)
    {
        // Do nothing by default.
    }
}
