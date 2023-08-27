using Boid.Utility;

namespace Boid.Simulation;

public interface IParameters
{
    Ref<float> MaxSpeed { get; }
    Ref<float> MinSpeed { get; }
    Ref<float> FlockDistance { get; }
    Ref<float> AvoidDistance { get; }
}

public class Parameters : IParameters
{
    public Ref<float> MaxSpeed { get; } = new(100);
    public Ref<float> MinSpeed { get; } = new(10);
    public Ref<float> FlockDistance { get; } = new(50);
    public Ref<float> AvoidDistance { get; } = new(10);
}
