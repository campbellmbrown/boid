namespace Boid;

public interface IFrameTickable
{
    void FrameTick(IFrameTickManager frameTickManager);
}
