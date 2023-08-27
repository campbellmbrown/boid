using Boid.Gui;
using Boid.Gui.Items;
using Boid.Input;
using Boid.Simulation;
using Boid.Visual;
using CreepyCrawler.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Boid;

public class Game1 : Game
{
    readonly GraphicsDeviceManager _graphics;
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

        Parameters parameters = new();
        Settings settings = new(
            _spriteBatchManager.GuiLayerView,
            GuiPlacement.TopLeft,
            contentProvider,
            _inputManager,
            parameters);
        _guiManager.AddItem(settings);
        _guiManager.FinalizeGui();

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
