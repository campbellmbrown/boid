using System;
using Boid.Visual;
using Microsoft.Xna.Framework;

namespace Boid.Gui.Components;

public interface ILabel : IGuiComponent
{
}

public class Label : GuiComponent, ILabel
{
    readonly ITextDisplay _text;

    public Label(HorizontalAlignment horizontalAlignment, ITextDisplay text)
        : base(horizontalAlignment)
    {
        _text = text;
        Width = (int)_text.Width;
        Height = (int)_text.Height;
    }

    public override void UpdatePosition(Vector2 position)
    {
        base.UpdatePosition(position);
        _text.UpdatePosition(Origin);
    }

    public override void Draw(ISpriteBatchWrapper spriteBatch)
    {
        base.Draw(spriteBatch);
        _text.Draw(spriteBatch);
    }
}
