using Microsoft.Xna.Framework;

namespace Boid;

public interface IFrameTickManager
{
    GameTime GameTime { set; }
    float TimeDiffSec { get; }
}

public class FrameTickManager : IFrameTickManager
{
    public GameTime? GameTime { private get; set; }
    public float TimeDiffSec => (GameTime == null) ? 0 : (float)GameTime.ElapsedGameTime.TotalSeconds;
}
