using Boid.Gui;

namespace Boid.Simulation;

public interface IParameters
{
    float MaxSpeed { get; }
    float MinSpeed { get; }
    float FlockDistance { get; }
    float AvoidDistance { get; }
}

public class Parameters : IParameters
{
    readonly INumericInput _maxSpeed;
    readonly INumericInput _minSpeed;
    readonly INumericInput _flockDistance;
    readonly INumericInput _avoidDistance;

    public Parameters(
        INumericInput maxSpeed,
        INumericInput minSpeed,
        INumericInput flockDistance,
        INumericInput avoidDistance

    )
    {
        _maxSpeed = maxSpeed;
        _minSpeed = minSpeed;
        _flockDistance = flockDistance;
        _avoidDistance = avoidDistance;
    }

    public float MaxSpeed => _maxSpeed.Value;
    public float MinSpeed => _minSpeed.Value;
    public float FlockDistance => _flockDistance.Value;
    public float AvoidDistance => _avoidDistance.Value;
}
