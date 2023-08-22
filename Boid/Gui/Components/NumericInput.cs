using System;
using Boid.Input;
using Boid.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Boid.Gui.Components;

public interface INumericInput : IGuiComponent, ILeftClickable
{
    float Value { get; set; }
}

public class NumericInput : GuiComponent, INumericInput
{
    readonly ITextDisplay _text;
    readonly HorizontalAlignment _horizontalAlignment;
    Vector2 _offset = Vector2.Zero;
    Vector2 _textOffset;
    Color _borderColor;

    const float cursorBlinkInterval = 0.5f;
    float _cursorBlinkTimer = 0f;
    bool _cursorVisible = false;

    const int spaceBetweenTextAndCursor = 1;
    Vector2 CursorTop => TextPosition + new Vector2(_text.WidthToIndex(cursorIndex) + spaceBetweenTextAndCursor, 0);
    Vector2 CursorBottom => TextPosition + new Vector2(_text.WidthToIndex(cursorIndex) + spaceBetweenTextAndCursor, _text.Height);
    int cursorIndex = 0;

    Vector2 Origin => Position + _offset;
    Vector2 TextPosition => Origin + _textOffset;

    public RectangleF LeftClickArea => new RectangleF(Origin.X, Origin.Y, Width, Height);

    bool _focused = false;
    public bool Focused
    {
        get => _focused;
        set
        {
            _focused = value;
            if (!_focused)
            {
                // Sync the value with the text when we lose focus
                Value = string.IsNullOrEmpty(_text.Text) ? 0f : float.Parse(_text.Text);
            }
        }
    }

    float _value;
    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            _text.Text = value.ToString();
            cursorIndex = _text.Text.Length;
        }
    }

    public NumericInput(
        ITextDisplay text,
        HorizontalAlignment horizontalAlignment,
        int width,
        int padding,
        float defaultValue)
    {
        _text = text;
        Value = defaultValue;
        _horizontalAlignment = horizontalAlignment;
        Width = width;
        Height = (int)(_text.Height + (2 * padding));

        // TODO: compare width to text width and raise a warning if it's too small.

        _textOffset = new Vector2(padding);
    }

    public override void FinalizeComponent(int availableWidth, int availableHeight)
    {
        base.FinalizeComponent(availableWidth, availableHeight);
        _offset = _horizontalAlignment switch
        {
            HorizontalAlignment.Left => Vector2.Zero,
            HorizontalAlignment.Center => new Vector2((availableWidth - Width) / 2f, 0f),
            HorizontalAlignment.Right => new Vector2(availableWidth - Width, 0f),
            _ => throw new ArgumentException("Horizontal alignment not supported."),
        };
    }

    public override void FrameTick(IFrameTickManager frameTickManager)
    {
        base.FrameTick(frameTickManager);
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
        base.Draw(spriteBatch);
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

    public void KeyPressed(Keys key)
    {
        switch (key)
        {
            case Keys.Escape:
                Value = _value; // Reset text display
                Focused = false;
                break;
            case Keys.Enter:
                Focused = false;
                break;
            case Keys.D0:
            case Keys.D1:
            case Keys.D2:
            case Keys.D3:
            case Keys.D4:
            case Keys.D5:
            case Keys.D6:
            case Keys.D7:
            case Keys.D8:
            case Keys.D9:
                _text.Text = _text.Text.Insert(cursorIndex, ((char)key).ToString());
                cursorIndex += 1;
                break;
            case Keys.Back:
            if (cursorIndex > 0)
                {
                    _text.Text = _text.Text.Remove(cursorIndex - 1, 1);
                    cursorIndex -= 1;
                }
                break;
            case Keys.Delete:
                if (cursorIndex < _text.Text.Length)
                {
                    _text.Text = _text.Text.Remove(cursorIndex, 1);
                }
                break;
            case Keys.OemPeriod:
                if (!_text.Text.Contains('.'))
                {
                    _text.Text = _text.Text.Insert(cursorIndex, ".");
                    cursorIndex += 1;
                }
                break;
            case Keys.Left:
                cursorIndex = Math.Max(0, cursorIndex - 1);
                _cursorBlinkTimer = 0f;
                _cursorVisible = true;
                break;
            case Keys.Right:
                cursorIndex = Math.Min(_text.Text.Length, cursorIndex + 1);
                _cursorBlinkTimer = 0f;
                _cursorVisible = true;
                break;
        }
    }
}
