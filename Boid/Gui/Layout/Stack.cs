using System.Collections.Generic;
using Boid.Visual;
using Microsoft.Xna.Framework;

namespace Boid.Gui.Layout;

public interface IStack : IGuiElement, IVisualRelative, IFrameTickable
{
    void AddComponent(IGuiComponent component);
    void FinalizeStack();
}

public abstract class Stack : IStack
{
    protected List<IGuiComponent> Components { get; } =  new();
    protected Vector2 Position { get; private set; } = Vector2.Zero;
    public int Width { get; protected set; }
    public int Height { get; protected set; }

    public void AddComponent(IGuiComponent component)
    {
        Components.Add(component);
    }

    public abstract void FinalizeStack();

    public virtual void UpdatePosition(Vector2 position)
    {
        Position = position;
    }

    public virtual void Draw(ISpriteBatchWrapper spriteBatch)
    {
        foreach (var component in Components)
        {
            component.Draw(spriteBatch);
        }
    }

    public virtual void FrameTick(IFrameTickManager frameTickManager)
    {
        foreach (var component in Components)
        {
            component.FrameTick(frameTickManager);
        }
    }
}
