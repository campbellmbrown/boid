using System;
using Boid.Input;
using Boid.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Boid.Gui;

public interface INumericInput : IGuiComponent, IFrameTickable, ILeftClickable
{
}

public class NumericInput : GuiComponent, INumericInput
{
    readonly ITextDisplay _text;
    readonly HorizontalAlignment _horizontalAlignment;
    Vector2 _offset = Vector2.Zero;
    Vector2 _textOffset;
    Color _borderColor;
    bool _finalized = false;

    const float cursorBlinkInterval = 0.5f;
    float _cursorBlinkTimer = 0f;
    bool _cursorVisible = false;

    Vector2 CursorTop => TextPosition + new Vector2(_text.Width, 0);
    Vector2 CursorBottom => TextPosition + new Vector2(_text.Width, _text.Height);

    Vector2 Origin => Position + _offset;
    Vector2 TextPosition => Origin + _textOffset;

    public RectangleF LeftClickArea => new RectangleF(Origin.X, Origin.Y, Width, Height);

    public bool Focused { get; set; }

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

    public override void FrameTick(IFrameTickManager frameTickManager)
    {
        if (Focused)
        {
            _cursorBlinkTimer += frameTickManager.TimeDiffSec;
            if (_cursorBlinkTimer >= cursorBlinkInterval)
            {
                _cursorBlinkTimer -= cursorBlinkInterval;
                _cursorVisible = !_cursorVisible;
            }
        }
        else
        {
            _cursorBlinkTimer = 0f;
            _cursorVisible = false;
        }
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
        if (_cursorVisible)
        {
            spriteBatch.SpriteBatch.DrawLine(CursorTop, CursorBottom, Color.White);
        }
        spriteBatch.SpriteBatch.DrawRectangle(LeftClickArea, _borderColor);
    }

    public void LeftClickAction()
    {
        Focused = true;
        _cursorVisible = true;
    }

    public void ChangeState(ClickState clickState)
    {
        _borderColor = clickState switch
        {
            ClickState.None => Color.Yellow,
            ClickState.Hovered => Color.Orange,
            ClickState.Clicked => Color.Magenta,
            _ => throw new ArgumentException("Click state not supported."),
        };
        if (Focused && (clickState == ClickState.None))
        {
            _borderColor = Color.Red;
        }
    }
}
