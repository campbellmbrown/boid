using System.Linq;
using Boid.Visual;
using Microsoft.Xna.Framework;

namespace Boid.Gui.Layout;

public interface IVerticalStack : IStack
{
}

public class VerticalStack : Stack, IVerticalStack
{
    readonly int _spacing;

    public VerticalStack(int spacing)
    {
        _spacing = spacing;
    }

    public override void FinalizeStack()
    {
        if (Components.Any())
        {
            Width = Components.Max(element => element.Width);
            Height = Components.Sum(element => element.Height) + (_spacing * (Components.Count - 1));
        }
        else
        {
            Width = 0;
            Height = 0;
        }
        foreach (var component in Components)
        {
            component.FinalizeComponent(Width, Height);
        }
    }

    public override void UpdatePosition(Vector2 position)
    {
        base.UpdatePosition(position);

        int heightOffset = 0;
        foreach (var component in Components)
        {
            component.UpdatePosition(position + new Vector2(0, heightOffset));
            heightOffset += component.Height;
            heightOffset += _spacing;
        }
    }

    public override void Draw(ISpriteBatchWrapper spriteBatch)
    {
        foreach (var component in Components)
        {
            component.Draw(spriteBatch);
        }
    }
}
