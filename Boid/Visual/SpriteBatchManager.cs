using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boid.Visual;

public enum DrawType
{
    Gui,
    Main,
}

public interface ISpriteBatchManager
{
    void Start(DrawType drawType);
    void Switch(DrawType drawType);
    void Clear();
    void Finish();

    ILayerView GuiLayerView { get; }
    ILayerView MainLayerView { get; }
}

public class SpriteBatchManager : ISpriteBatchManager
{
    readonly GraphicsDevice _graphicsDevice;
    readonly ISpriteBatchWrapper _wrapper;

    public ILayerView GuiLayerView { get; private set; }
    public ILayerView MainLayerView { get; private set; }

    /* Render targets
    *
    * Instead of drawing our sprites to the back buffer we can instruct the GraphicsDevice to draw
    * to a render target instead. A render target is essentially an image that we are drawing to,
    * and then when we are done drawing to that render target we can draw it to the back buffer like
    * a regular texture. This is useful for:
    *
    * - Applying effects to a specific layer
    * - Applying global effects to all the layers (or specific layers) when we draw to the back buffer
    * - Having different cameras for each layer (e.g. a menu overlay vs game content)
    */

    /// <summary>
    /// Render target for content that doesn't move with the player/game camera.
    /// </summary>
    readonly RenderTarget2D _guiRenderTarget;

    /// <summary>
    /// Render target for the main content.
    /// </summary>
    readonly RenderTarget2D _mainRenderTarget;

    public SpriteBatchManager(GraphicsDevice graphicsDevice, ISpriteBatchWrapper spriteBatchWrapper)
    {
        _graphicsDevice = graphicsDevice;
        _wrapper = spriteBatchWrapper;

        PresentationParameters pp = _graphicsDevice.PresentationParameters;
        _guiRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        _mainRenderTarget = new RenderTarget2D(_graphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        GuiLayerView = new LayerView(_graphicsDevice, 2);
        MainLayerView = new LayerView(_graphicsDevice, 4);
    }

    public void Start(DrawType drawType)
    {
        switch (drawType)
        {
            case DrawType.Gui:
                _graphicsDevice.SetRenderTarget(_guiRenderTarget);
                _graphicsDevice.Clear(Color.Transparent);
                _wrapper.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: GuiLayerView.Camera.GetViewMatrix());
                break;
            case DrawType.Main:
                _graphicsDevice.SetRenderTarget(_mainRenderTarget);
                _graphicsDevice.Clear(Color.Transparent);
                _wrapper.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, transformMatrix: MainLayerView.Camera.GetViewMatrix());
                break;
        }
    }

    public void Switch(DrawType drawType)
    {
        _wrapper.SpriteBatch.End();
        Start(drawType);
    }

    public void Clear() => _graphicsDevice.Clear(Color.Black);

    public void Finish()
    {
        _wrapper.SpriteBatch.End();

        _graphicsDevice.SetRenderTarget(null);
        _graphicsDevice.Clear(Color.Black);

        // (1) Draw the main content to the temporary target with the point light as a mask.
        _wrapper.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _wrapper.SpriteBatch.Draw(_mainRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _wrapper.SpriteBatch.End();

        // (2) Draw the rest of the sprite batches to the back buffer.
        _wrapper.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _wrapper.SpriteBatch.Draw(_guiRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        _wrapper.SpriteBatch.End();
    }
}
