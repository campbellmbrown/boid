using Boid.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Boid.Visual;

public interface ILayerView
{
    OrthographicCamera Camera { get; }

    /// <summary>
    /// Top left of the view.
    /// </summary>
    Vector2 Origin { get; }

    /// <summary>
    /// Width/Height of the view.
    /// </summary>
    Vector2 Size { get; }

    Vector2 MousePosition { get; }

    float Zoom { get; }

    void Focus(Vector2 focusPoint);
}

public class LayerView : ILayerView
{
    readonly GraphicsDevice _graphicsDevice;

    public OrthographicCamera Camera { get; private set; }
    public Vector2 Origin => Camera.ScreenToWorld(Vector2.Zero);
    public Vector2 Size => new Vector2(_graphicsDevice.Viewport.Bounds.Width, _graphicsDevice.Viewport.Bounds.Height) / Camera.Zoom;
    public Vector2 MousePosition
    {
        get
        {
            Vector2 mousePos = Conversion.PointToVector2(Mouse.GetState().Position);
            return Camera.ScreenToWorld(mousePos.X, mousePos.Y);
        }
    }

    const int SCALE_FACTOR = 1;

    public float Zoom { get; init; }

    public LayerView(GraphicsDevice graphicsDevice, float zoom)
    {
        _graphicsDevice = graphicsDevice;
        Zoom = zoom;

        // Each layer has a different camera because they can have different positions/zooms.
        Camera = new OrthographicCamera(graphicsDevice);

        float zoomAdjustment = Zoom - SCALE_FACTOR;
        Camera.ZoomIn(zoomAdjustment);
    }

    public void Focus(Vector2 focusPoint) => Camera.LookAt(focusPoint);
}
