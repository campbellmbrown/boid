using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace Boid.Visual;

public interface ITextDisplay : IVisualRelative
{
    float Width { get; }
    float Height { get; }
    Vector2 Size { get; }

    void UpdateText(string text);
}

public class TextDisplay : ITextDisplay
{
    readonly Color _color;
    readonly BitmapFont _font;
    readonly float _scale;

    string _text;

    Vector2 _position = Vector2.Zero;

    public float Width => _font.MeasureString(_text).Width * _scale;
    public float Height => _font.LineHeight * _scale;
    public Vector2 Size => new(Width, Height);

    public TextDisplay(string text, BitmapFont font, Color color, float scale)
    {
        _text = text;
        _color = color;
        _font = font;
        _scale = scale;
    }

    public void UpdateText(string text)
    {
        _text = text;
    }

    public void UpdatePosition(Vector2 position)
    {
        _position = position;
    }

    public void Draw(ISpriteBatchWrapper spriteBatch)
    {
        spriteBatch.SpriteBatch.DrawString(_font, _text, _position, _color, _scale);
    }
}
