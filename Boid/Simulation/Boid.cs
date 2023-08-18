using System;
using System.Collections.Generic;
using Boid.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Boid.Simulation;

public record Trace : IFrameTickable, IVisual
{
    readonly Vector2 _pointA;
    readonly Vector2 _pointB;

    const float _timeToDisappear = 10f;
    float _fade = 1f;

    public Trace(Vector2 pointA, Vector2 pointB)
    {
        _pointA = pointA;
        _pointB = pointB;
    }

    public bool Expired { get; private set; } = false;

    public void Draw(ISpriteBatchWrapper spriteBatch)
    {
        spriteBatch.SpriteBatch.DrawLine(_pointA, _pointB, Color.Purple * 0.5f * _fade, _fade, 0.1f);
    }

    public void FrameTick(IFrameTickManager frameTickManager)
    {
        _fade -= frameTickManager.TimeDiffSec / _timeToDisappear;
        if (_fade <= 0f)
        {
            _fade = 0f;
            Expired = true;
        }
    }
}

public record Boid : IVisual, IFrameTickable
{
    readonly IParameters _parameters;
    readonly ILayerView _layerView;

    Vector2 _lastTrace;
    public List<Trace> _traces = new();
    float _traceTimer = 0f;

    public Boid(IParameters parameters, ILayerView layerView, Vector2 velocity)
    {
        _parameters = parameters;
        _layerView = layerView;
        Velocity = velocity;
        _lastTrace = Position;
    }

    public Vector2 Position { get; set; }
    public Vector2 FlockVelocity { get; set; }
    public Vector2 AlignVelocity { get; set; }
    public Vector2 AvoidVelocity { get; set; }
    public Vector2 PredatorVelocity { get; set; }
    public Vector2 Velocity { get; private set; }

    public void FrameTick(IFrameTickManager frameTickManager)
    {
        Velocity += FlockVelocity + AlignVelocity + AvoidVelocity + PredatorVelocity;
        AvoidWalls();

        var speed = Velocity.Length();
        if (speed > _parameters.MaxSpeed)
        {
            Velocity = Vector2.Normalize(Velocity) * _parameters.MaxSpeed;
        }
        else if (speed < _parameters.MinSpeed)
        {
            Velocity = Vector2.Normalize(Velocity) * _parameters.MinSpeed;
        }
        Position += Velocity * frameTickManager.TimeDiffSec;

        _traceTimer += frameTickManager.TimeDiffSec;
        const float timeBetweenTraces = 0.2f;
        if (_traceTimer >= timeBetweenTraces)
        {
            _traceTimer -= timeBetweenTraces;
            _traces.Add(new Trace(Position, _lastTrace));
            _lastTrace = Position;
        }
        _traces.RemoveAll(trace => trace.Expired);

        foreach (var trace in _traces)
        {
            trace.FrameTick(frameTickManager);
        }
    }

    void AvoidWalls()
    {
        Vector2 velocity = Velocity;
        if (Position.X >= _layerView.Origin.X + _layerView.Size.X - 20)
        {
            velocity.X -= 1;
        }
        else if (Position.X <= _layerView.Origin.X + 20)
        {
            velocity.X += 1;
        }
        if (Position.Y >= _layerView.Origin.Y + _layerView.Size.Y - 20)
        {
            velocity.Y -= 1;
        }
        else if (Position.Y <= _layerView.Origin.Y + 20)
        {
            velocity.Y += 1;
        }
        Velocity = velocity;
    }

    public void Draw(ISpriteBatchWrapper spriteBatch)
    {
        spriteBatch.SpriteBatch.DrawCircle(Position, 3f, 20, Color.White, 1f, 0.5f);
        // spriteBatch.SpriteBatch.DrawCircle(Position, _parameters.FlockDistance, 50, Color.White * 0.1f, 0.25f);
        // spriteBatch.SpriteBatch.DrawCircle(Position, _parameters.AvoidDistance, 50, Color.White * 0.1f, 0.25f);
        spriteBatch.SpriteBatch.DrawLine(Position, Position + (FlockVelocity * 30f), Color.Orange, 0.25f, 0.9f);
        spriteBatch.SpriteBatch.DrawLine(Position, Position + (AlignVelocity * 30f), Color.Cyan, 0.25f, 0.9f);
        spriteBatch.SpriteBatch.DrawLine(Position, Position + (AvoidVelocity * 30f), Color.Green, 0.25f, 0.9f);
        spriteBatch.SpriteBatch.DrawLine(Position, Position + (PredatorVelocity * 30f), Color.Yellow, 0.25f, 0.9f);

        foreach (var trace in _traces)
        {
            trace.Draw(spriteBatch);
        }
    }
}
