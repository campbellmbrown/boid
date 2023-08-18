using System;
using System.Collections.Generic;
using Boid.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Boid.Simulation;

public class BoidSimulator : IVisual, IFrameTickable
{

    readonly IParameters _parameters;
    readonly ILayerView _layerView;
    readonly List<Boid> _boids = new();
    Random _random = new();

    const int _count = 200;

    public BoidSimulator(IParameters parameters, ILayerView layerView)
    {
        _parameters = parameters;
        _layerView = layerView;
        for (int idx = 0; idx < _count; idx++)
        {
            _boids.Add(new Boid(_parameters, _layerView, new Vector2(_random.Next(-5, 5), _random.Next(-5, 5))));
        }
    }

    public void FrameTick(IFrameTickManager frameTickManager)
    {
        foreach (var boid in _boids)
        {
            boid.FlockVelocity = Flock(boid, 0.003f);
            boid.AlignVelocity = Align(boid, 50, 0.01f);
            boid.AvoidVelocity = Avoid(boid, 0.01f);
            boid.PredatorVelocity = Predator(boid, 80, 0.01f);
            boid.FrameTick(frameTickManager);
        }
    }

    public void Draw(ISpriteBatchWrapper spriteBatch)
    {
        foreach (var boid in _boids)
        {
            boid.Draw(spriteBatch);
        }
        spriteBatch.SpriteBatch.DrawCircle(_layerView.MousePosition, 80, 50, Color.White * 0.1f, 0.25f);
    }

    Vector2 Flock(Boid boid, float power)
    {
        Vector2 meanPosition = boid.Position;
        foreach (var other in _boids)
        {
            if (other == boid)
            {
                continue;
            }
            var difference = other.Position - boid.Position;
            if (difference.Length() < _parameters.FlockDistance)
            {
                meanPosition += other.Position;
            }
        }
        meanPosition /= _boids.Count - 1;
        Vector2 deltaCenter = meanPosition - boid.Position;
        return deltaCenter * power;
    }

    Vector2 Avoid(Boid boid, float power)
    {
        Vector2 closenessSum = Vector2.Zero;
        foreach (var other in _boids)
        {
            if (other == boid)
            {
                continue;
            }
            var difference = other.Position - boid.Position;
            if (difference.Length() < _parameters.AvoidDistance)
            {
                var closeness = _parameters.AvoidDistance - difference.Length();
                closenessSum += -difference * closeness;
            }
        }
        return closenessSum * power;
    }

    Vector2 Align(Boid boid, float distance, float power)
    {
        Vector2 meanVelocity = boid.Velocity;
        foreach (var other in _boids)
        {
            if (other == boid)
            {
                continue;
            }
            var difference = other.Position - boid.Position;
            if (difference.Length() < distance)
            {
                meanVelocity += other.Velocity;
            }
        }
        meanVelocity /= _boids.Count - 1;
        Vector2 deltaVelocity = meanVelocity - boid.Velocity;
        return deltaVelocity * power;
    }

    Vector2 Predator(Boid boid, float distance, float power)
    {
        Vector2 velocity = Vector2.Zero;
        Vector2 predator = _layerView.MousePosition;
        var difference = predator - boid.Position;
        if (difference.Length() < distance)
        {
            float closeness = distance - difference.Length();
            velocity = -Vector2.Normalize(difference) * closeness;
        }
        return velocity * power;
    }
}
