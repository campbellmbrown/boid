using Boid.Gui;
using Boid.Gui.Items;
using Boid.Gui.Layout;
using Boid.Input;
using Boid.Simulation;
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
    BoidSimulator? _boidSimulator;

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
        _spriteBatchManager.MainLayerView.Camera.LookAt(Vector2.Zero);
        ContentProvider contentProvider = new(Content);
        MouseWrapper mouseWrapper = new();
        KeyboardWrapper keyboardWrapper = new();
        _inputManager = new InputManager(_spriteBatchManager.GuiLayerView, mouseWrapper, keyboardWrapper);

        const int padding = 4;
        const int spacing = 10;
        const int margin = 10;
        const int inputWidth = 100;
        var font = contentProvider.GetFont(FontId.Normal);
        NumericInput maxSpeedInput = new(new TextDisplay(font, Color.White, 1f), HorizontalAlignment.Left, inputWidth, padding, 100f);
        NumericInput minSpeedInput = new(new TextDisplay(font, Color.White, 1f), HorizontalAlignment.Left, inputWidth, padding, 10f);
        NumericInput flockDistanceInput = new(new TextDisplay(font, Color.White, 1f), HorizontalAlignment.Left, inputWidth, padding, 50f);
        NumericInput avoidDistanceInput = new(new TextDisplay(font, Color.White, 1f), HorizontalAlignment.Left, inputWidth, padding, 10f);
        Label maxSpeedLabel = new(new TextDisplay("Max speed", font, Color.White, 1f), HorizontalAlignment.Left);
        Label minSpeedLabel = new(new TextDisplay("Min speed", font, Color.White, 1f), HorizontalAlignment.Left);
        Label flockDistanceLabel = new(new TextDisplay("Flock distance", font, Color.White, 1f), HorizontalAlignment.Left);
        Label avoidDistanceLabel = new(new TextDisplay("Avoid distance", font, Color.White, 1f), HorizontalAlignment.Left);
        Grid grid = new(4, 2, spacing, margin);
        grid.AddComponent(maxSpeedLabel, 0, 0);
        grid.AddComponent(maxSpeedInput, 0, 1);
        grid.AddComponent(minSpeedLabel, 1, 0);
        grid.AddComponent(minSpeedInput, 1, 1);
        grid.AddComponent(flockDistanceLabel, 2, 0);
        grid.AddComponent(flockDistanceInput, 2, 1);
        grid.AddComponent(avoidDistanceLabel, 3, 0);
        grid.AddComponent(avoidDistanceInput, 3, 1);
        Settings settings = new(_spriteBatchManager.GuiLayerView, grid, GuiPlacement.TopLeft);

        _guiManager.AddItem(settings);
        _guiManager.FinalizeGui();

        _inputManager.RegisterLeftClick(maxSpeedInput);
        _inputManager.RegisterLeftClick(minSpeedInput);
        _inputManager.RegisterLeftClick(flockDistanceInput);
        _inputManager.RegisterLeftClick(avoidDistanceInput);

        Parameters parameters = new(
            maxSpeedInput,
            minSpeedInput,
            flockDistanceInput,
            avoidDistanceInput
        );
        _boidSimulator = new BoidSimulator(parameters, _spriteBatchManager.MainLayerView);
    }

    protected override void Update(GameTime gameTime)
    {
        _frameTickManager.GameTime = gameTime;
        _guiManager.FrameTick(_frameTickManager);
        _inputManager!.FrameTick(_frameTickManager);
        _boidSimulator!.FrameTick(_frameTickManager);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _spriteBatchManager!.Start(DrawType.Gui);
        _guiManager.Draw(_spriteBatchWrapper!);
        _spriteBatchManager!.Switch(DrawType.Main);
        _boidSimulator!.Draw(_spriteBatchWrapper!);
        _spriteBatchManager.Finish();
        base.Draw(gameTime);
    }
}
