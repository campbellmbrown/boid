using System;
using Boid.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Boid.Gui;

public interface INumericInput : IGuiComponent
{
}

public class NumericInput : GuiComponent, INumericInput
{
    readonly ITextDisplay _text;
    readonly HorizontalAlignment _horizontalAlignment;
    Vector2 _offset = Vector2.Zero;
    Vector2 _textOffset;
    bool _finalized = false;

    Vector2 Origin => Position + _offset;
    Vector2 TextPosition => Origin + _textOffset;

    public NumericInput(ITextDisplay text, HorizontalAlignment horizontalAlignment, int width, int padding)
    {
        _text = text;
        _horizontalAlignment = horizontalAlignment;
        Width = width;
        Height = (int)(_text.Height + (2 * padding));

        // TODO: compare width to text width and raise a warning if it's too small.

        _textOffset = new Vector2(padding);
    }

    public override void FinalizeComponent(int availableWidth, int availableHeight)
    {
        if (_finalized)
        {
            throw new InvalidOperationException("Attempted to finalize numeric input when already finalized.");
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
        _text.UpdatePosition(TextPosition);
    }

    public override void Draw(ISpriteBatchWrapper spriteBatch)
    {
        if (!_finalized)
        {
            throw new InvalidOperationException("Attempted to draw numeric input before finalizing.");
        }
        _text.Draw(spriteBatch);
        spriteBatch.SpriteBatch.DrawRectangle(new RectangleF(Origin.X, Origin.Y, Width, Height), Color.Red);
    }
}
