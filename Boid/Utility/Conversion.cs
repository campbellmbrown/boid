using Microsoft.Xna.Framework;

namespace Boid.Utility;

public static class Conversion
{
    public static Vector2 PointToVector2(Point point) => new Vector2(point.X, point.Y);
}
