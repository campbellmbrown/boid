using Boid.Gui;
using Boid.Gui.Layout;
using Boid.Input;
using Boid.Visual;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Boid;

public class Game1 : Game
{
    GraphicsDeviceManager _graphics;
    readonly GuiManager _guiManager = new();
    readonly FrameTickManager _frameTickManager = new();
    SpriteBatchWrapper? _spriteBatchWrapper;
    SpriteBatchManager? _spriteBatchManager;

    IInputManager? _inputManager;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
    }

    protected override void Initialize()
    {
        _graphics.IsFullScreen = false;
        _graphics.SynchronizeWithVerticalRetrace = true;
        Window.AllowUserResizing = true;
        _graphics.ApplyChanges();

        IsMouseVisible = true;
        IsFixedTimeStep = true;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        SpriteBatch spriteBatch = new(GraphicsDevice);
        _spriteBatchWrapper = new SpriteBatchWrapper(spriteBatch);
        _spriteBatchManager = new SpriteBatchManager(GraphicsDevice, _spriteBatchWrapper);
        ContentProvider contentProvider = new(Content);
        MouseWrapper mouseWrapper = new();
        _inputManager = new InputManager(_spriteBatchManager.GuiLayerView, mouseWrapper);

        const int padding = 4;
        const int spacing = 10;
        VerticalStack verticalStack = new(spacing);
        NumericInput testNumericInput1 = new(
            new TextDisplay(contentProvider.GetFont(FontId.Normal), Color.White, 1f),
            HorizontalAlignment.Right, 100, padding, 10.0f);
        NumericInput testNumericInput2 = new(
            new TextDisplay(contentProvider.GetFont(FontId.Normal), Color.White, 1f),
            HorizontalAlignment.Center, 200, padding, 10.1f);
        NumericInput testNumericInput3 = new(
            new TextDisplay(contentProvider.GetFont(FontId.Normal), Color.White, 1f),
            HorizontalAlignment.Left, 300, padding, 0.1f);
        NumericInput testNumericInput4 = new(
            new TextDisplay(contentProvider.GetFont(FontId.Normal), Color.White, 1f),
            HorizontalAlignment.Left, 50, padding, -10.1f);
        verticalStack.AddComponent(testNumericInput1);
        verticalStack.AddComponent(testNumericInput2);
        verticalStack.AddComponent(testNumericInput3);
        verticalStack.AddComponent(testNumericInput4);
        Settings settings = new(_spriteBatchManager.GuiLayerView, verticalStack, GuiPlacement.TopLeft);

        _guiManager.AddItem(settings);
        _guiManager.FinalizeGui();

        _inputManager.RegisterLeftClick(testNumericInput1);
        _inputManager.RegisterLeftClick(testNumericInput2);
        _inputManager.RegisterLeftClick(testNumericInput3);
        _inputManager.RegisterLeftClick(testNumericInput4);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        _frameTickManager.GameTime = gameTime;
        _guiManager.FrameTick(_frameTickManager);
        _clickManager!.FrameTick(_frameTickManager);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _spriteBatchManager!.Start(DrawType.Gui);
        _guiManager.Draw(_spriteBatchWrapper!);
        _spriteBatchManager.Finish();
        base.Draw(gameTime);
    }
}
