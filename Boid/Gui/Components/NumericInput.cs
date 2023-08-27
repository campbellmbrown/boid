using System;
using Boid.Input;
using Boid.Utility;
using Boid.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Boid.Gui.Components;

public interface INumericInput : IGuiComponent, ILeftClickable
{
}

public class NumericInput : GuiComponent, INumericInput
{
    readonly ITextDisplay _text;
    readonly Ref<float> _ref;
    Vector2 _textOffset;
    Color _borderColor;

    const float cursorBlinkInterval = 0.5f;
    float _cursorBlinkTimer = 0f;

    const int spaceBetweenTextAndCursor = 1;
    Vector2 CursorTop => TextPosition + new Vector2(_text.WidthToIndex(CursorIndex) + spaceBetweenTextAndCursor, 0);
    Vector2 CursorBottom => TextPosition + new Vector2(_text.WidthToIndex(CursorIndex) + spaceBetweenTextAndCursor, _text.Height);

    Vector2 TextPosition => Origin + _textOffset;

    public NumericInput(ITextDisplay text, int width, int padding, Ref<float> value)
        : this(HorizontalAlignment.Left, text, width, padding, value)
    {
    }

    public NumericInput(
        HorizontalAlignment horizontalAlignment,
        ITextDisplay text,
        int width,
        int padding,
        Ref<float> value)
        : base(horizontalAlignment)
    {
        _text = text;
        _ref = value;
        SyncTextWithValue();
        Width = width;
        Height = (int)(_text.Height + (2 * padding));

        // TODO: compare width to text width and raise a warning if it's too small.

        _textOffset = new Vector2(padding);
    }

    public RectangleF LeftClickArea => new(Origin.X, Origin.Y, Width, Height);

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
                UpdateValue(string.IsNullOrEmpty(_text.Text) ? 0f : float.Parse(_text.Text));
            }
        }
    }

    public int CursorIndex { get; private set; } = 0;
    public bool CursorVisible { get; private set; } = false;

    public override void FrameTick(IFrameTickManager frameTickManager)
    {
        base.FrameTick(frameTickManager);
        if (Focused)
        {
            _cursorBlinkTimer += frameTickManager.TimeDiffSec;
            if (_cursorBlinkTimer >= cursorBlinkInterval)
            {
                _cursorBlinkTimer -= cursorBlinkInterval;
                CursorVisible = !CursorVisible;
            }
        }
        else
        {
            _cursorBlinkTimer = 0f;
            CursorVisible = false;
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
        if (CursorVisible)
        {
            spriteBatch.SpriteBatch.DrawLine(CursorTop, CursorBottom, Color.White);
        }
        spriteBatch.SpriteBatch.DrawRectangle(LeftClickArea, _borderColor);
    }

    public void LeftClickAction()
    {
        Focused = true;
        CursorVisible = true;
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
                SyncTextWithValue();
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
                _text.Text = _text.Text.Insert(CursorIndex, ((char)key).ToString());
                CursorIndex += 1;
                break;
            case Keys.Back:
            if (CursorIndex > 0)
                {
                    _text.Text = _text.Text.Remove(CursorIndex - 1, 1);
                    CursorIndex -= 1;
                }
                break;
            case Keys.Delete:
                if (CursorIndex < _text.Text.Length)
                {
                    _text.Text = _text.Text.Remove(CursorIndex, 1);
                }
                break;
            case Keys.OemPeriod:
                if (!_text.Text.Contains('.'))
                {
                    _text.Text = _text.Text.Insert(CursorIndex, ".");
                    CursorIndex += 1;
                }
                break;
            case Keys.Left:
                CursorIndex = Math.Max(0, CursorIndex - 1);
                _cursorBlinkTimer = 0f;
                CursorVisible = true;
                break;
            case Keys.Right:
                CursorIndex = Math.Min(_text.Text.Length, CursorIndex + 1);
                _cursorBlinkTimer = 0f;
                CursorVisible = true;
                break;
        }
    }

    void UpdateValue(float value)
    {
        _ref.Value = value;
        SyncTextWithValue();
    }

    void SyncTextWithValue()
    {
        _text.Text = _ref.Value.ToString();
        CursorIndex = _text.Text.Length;
    }
}
