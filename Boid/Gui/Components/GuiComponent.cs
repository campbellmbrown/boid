using System;
using Boid.Visual;
using Microsoft.Xna.Framework;

namespace Boid.Gui.Components;

public interface IGuiComponent : IGuiElement, IVisualRelative, IFrameTickable
{
    Vector2 Position { get; }
    void FinalizeComponent(int availableWidth, int availableHeight);
}

public abstract class GuiComponent : IGuiComponent
{
    bool _finalized = false;

    public Vector2 Position { get; protected set; } = Vector2.Zero;
    public int Width { get; protected init; }
    public int Height { get; protected init; }

    public virtual void FinalizeComponent(int availableWidth, int availableHeight)
    {
        if (_finalized)
        {
            throw new InvalidOperationException("Attempted to finalize GUI component when already finalized.");
        }
        _finalized = true;
    }

    public virtual void UpdatePosition(Vector2 position)
    {
        Position = position;
    }

    public virtual void Draw(ISpriteBatchWrapper spriteBatch)
    {
        if (!_finalized)
        {
            throw new InvalidOperationException("Attempted to draw GUI component before finalizing.");
        }
    }

    public virtual void FrameTick(IFrameTickManager frameTickManager)
    {
        if (!_finalized)
        {
            throw new InvalidOperationException("Attempted to run GUI component before finalizing.");
        }
    }
}
