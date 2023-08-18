using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace Boid.Visual;

public interface ITextDisplay : IVisualRelative
{
    string Text { get; set; }
    float Width { get; }
    float Height { get; }
    Vector2 Size { get; }

    public float WidthToIndex(int idx);
}

public class TextDisplay : ITextDisplay
{
    readonly Color _color;
    readonly BitmapFont _font;
    readonly float _scale;

    Vector2 _position = Vector2.Zero;

    public string Text { get; set; }
    public float Width => _font.MeasureString(Text).Width * _scale;
    public float Height => _font.LineHeight * _scale;
    public Vector2 Size => new(Width, Height);

    public TextDisplay(string text, BitmapFont font, Color color, float scale)
    {
        Text = text;
        _color = color;
        _font = font;
        _scale = scale;
    }

    public TextDisplay(BitmapFont font, Color color, float scale)
        : this("", font, color, scale)
    {
    }

    public void UpdatePosition(Vector2 position)
    {
        _position = position;
    }

    public void Draw(ISpriteBatchWrapper spriteBatch)
    {
        spriteBatch.SpriteBatch.DrawString(_font, Text, _position, _color, _scale);
    }

    public float WidthToIndex(int idx) => _font.MeasureString(Text[..idx]).Width * _scale;
}
