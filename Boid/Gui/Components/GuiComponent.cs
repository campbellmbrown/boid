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
    readonly HorizontalAlignment _horizontalAlignment;
    Vector2 _offset = Vector2.Zero;
    bool _finalized = false;

    protected GuiComponent(HorizontalAlignment horizontalAlignment)
    {
        _horizontalAlignment = horizontalAlignment;
    }

    public Vector2 Position { get; protected set; } = Vector2.Zero;
    public int Width { get; protected init; }
    public int Height { get; protected init; }

    protected Vector2 Origin => Position + _offset;

    public virtual void FinalizeComponent(int availableWidth, int availableHeight)
    {
        if (_finalized)
        {
            throw new InvalidOperationException("Attempted to finalize GUI component when already finalized.");
        }
        _offset = _horizontalAlignment switch
        {
            HorizontalAlignment.Left => Vector2.Zero,
            HorizontalAlignment.Center => new Vector2((availableWidth - Width) / 2f, 0f),
            HorizontalAlignment.Right => new Vector2(availableWidth - Width, 0f),
            _ => throw new ArgumentException("Horizontal alignment not supported."),
        };
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
