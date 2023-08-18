using Microsoft.Xna.Framework;

namespace Boid.Visual;

public interface IVisual
{
    void Draw(ISpriteBatchWrapper spriteBatch);
}

/// <summary>
/// For when a visual element doesn't have an internal position to draw itself at.
/// </summary>
public interface IVisualRelative : IVisual
{
    void UpdatePosition(Vector2 position);
}
