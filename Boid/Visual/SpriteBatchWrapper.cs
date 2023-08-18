using Microsoft.Xna.Framework.Graphics;

namespace Boid.Visual;

public interface ISpriteBatchWrapper
{
    SpriteBatch SpriteBatch { get; }
}

public class SpriteBatchWrapper : ISpriteBatchWrapper
{
    public SpriteBatchWrapper(SpriteBatch spriteBatch)
    {
        SpriteBatch = spriteBatch;
    }

    public SpriteBatch SpriteBatch { get; init; }
}
