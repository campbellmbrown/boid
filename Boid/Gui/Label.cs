using System;
using Boid.Visual;
using Microsoft.Xna.Framework;

namespace Boid.Gui;

public interface ILabel : IGuiComponent
{
}

public class Label : GuiComponent, ILabel
{
    readonly ITextDisplay _text;
    readonly HorizontalAlignment _horizontalAlignment;
    Vector2 _offset = Vector2.Zero;
    bool _finalized = false;

    Vector2 Origin => Position + _offset;

    public Label(ITextDisplay text, HorizontalAlignment horizontalAlignment)
    {
        _text = text;
        _horizontalAlignment = horizontalAlignment;
        Width = (int)_text.Width;
        Height = (int)_text.Height;
    }

    public override void FinalizeComponent(int availableWidth, int availableHeight)
    {
        if (_finalized)
        {
            throw new InvalidOperationException("Attempted to finalize label when already finalized.");
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

    public override void UpdatePosition(Vector2 position)
    {
        base.UpdatePosition(position);
        _text.UpdatePosition(Origin);
    }

    public override void Draw(ISpriteBatchWrapper spriteBatch)
    {
        if (!_finalized)
        {
            throw new InvalidOperationException("Attempted to draw label before finalizing.");
        }
        _text.Draw(spriteBatch);
    }
}
