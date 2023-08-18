using Boid.Gui;
using Boid.Gui.Layout;
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

        const int padding = 4;
        const int spacing = 10;
        VerticalStack verticalStack = new(spacing);
        NumericInput testNumericInput1 = new(
            new TextDisplay("0", contentProvider.GetFont(FontId.Normal), Color.White, 1f),
            HorizontalAlignment.Right, 100, padding);
        NumericInput testNumericInput2 = new(
            new TextDisplay("0", contentProvider.GetFont(FontId.Normal), Color.White, 1f),
            HorizontalAlignment.Center, 200, padding);
        NumericInput testNumericInput3 = new(
            new TextDisplay("0", contentProvider.GetFont(FontId.Normal), Color.White, 1f),
            HorizontalAlignment.Left, 300, padding);
        NumericInput testNumericInput4 = new(
            new TextDisplay("0", contentProvider.GetFont(FontId.Normal), Color.White, 1f),
            HorizontalAlignment.Left, 50, padding);
        verticalStack.AddComponent(testNumericInput1);
        verticalStack.AddComponent(testNumericInput2);
        verticalStack.AddComponent(testNumericInput3);
        verticalStack.AddComponent(testNumericInput4);
        Settings settings = new(_spriteBatchManager.GuiLayerView, verticalStack, GuiPlacement.TopLeft);

        _guiManager.AddItem(settings);
        _guiManager.FinalizeGui();
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        _frameTickManager.GameTime = gameTime;
        _guiManager.FrameTick(_frameTickManager);
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
