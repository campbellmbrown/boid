using Boid.Gui;
using Boid.Gui.Layout;
using Boid.Input;
using Boid.Visual;
using CreepyCrawler.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        Vector2 screenSize = new(
            GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
            GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

        // Remove the scalars when switching to full screen.
        _graphics.PreferredBackBufferWidth = (int)(screenSize.X * 0.95f);
        _graphics.PreferredBackBufferHeight = (int)(screenSize.Y * 0.8f);
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
        KeyboardWrapper keyboardWrapper = new();
        _inputManager = new InputManager(_spriteBatchManager.GuiLayerView, mouseWrapper, keyboardWrapper);

        const int padding = 4;
        const int spacing = 10;
        const int margin = 10;
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
        Grid grid = new(4, 2, spacing, margin);
        grid.AddComponent(testNumericInput1, 0, 0);
        grid.AddComponent(testNumericInput2, 1, 1);
        grid.AddComponent(testNumericInput3, 2, 0);
        grid.AddComponent(testNumericInput4, 3, 0);
        Settings settings = new(_spriteBatchManager.GuiLayerView, grid, GuiPlacement.TopLeft);

        _guiManager.AddItem(settings);
        _guiManager.FinalizeGui();

        _inputManager.RegisterLeftClick(testNumericInput1);
        _inputManager.RegisterLeftClick(testNumericInput2);
        _inputManager.RegisterLeftClick(testNumericInput3);
        _inputManager.RegisterLeftClick(testNumericInput4);
    }

    protected override void Update(GameTime gameTime)
    {
        _frameTickManager.GameTime = gameTime;
        _guiManager.FrameTick(_frameTickManager);
        _inputManager!.FrameTick(_frameTickManager);
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
