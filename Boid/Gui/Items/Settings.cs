using Boid.Gui.Components;
using Boid.Gui.Layout;
using Boid.Input;
using Boid.Simulation;
using Boid.Visual;
using Microsoft.Xna.Framework;

namespace Boid.Gui.Items;

public interface ISettings : IGuiItem
{
}

public class Settings : GuiItem, ISettings
{
    readonly IGrid _grid;

    public Settings(
        ILayerView layerView,
        GuiPlacement placement,
        IContentProvider contentProvider,
        IInputManager inputManager,
        Parameters parameters)
        : base(layerView, placement)
    {
        const int padding = 4;
        const int spacing = 10;
        const int margin = 10;
        const int inputWidth = 100;
        var font = contentProvider.GetFont(FontId.Normal);

        NumericInput maxSpeedInput = new(new TextDisplay(font, Color.White, 1f), inputWidth, padding, parameters.MaxSpeed);
        NumericInput minSpeedInput = new(new TextDisplay(font, Color.White, 1f), inputWidth, padding, parameters.MinSpeed);
        NumericInput flockDistanceInput = new(new TextDisplay(font, Color.White, 1f), inputWidth, padding, parameters.FlockDistance);
        NumericInput avoidDistanceInput = new(new TextDisplay(font, Color.White, 1f), inputWidth, padding, parameters.AvoidDistance);
        Label maxSpeedLabel = new(new TextDisplay("Max speed", font, Color.White, 1f));
        Label minSpeedLabel = new(new TextDisplay("Min speed", font, Color.White, 1f));
        Label flockDistanceLabel = new(new TextDisplay("Flock distance", font, Color.White, 1f));
        Label avoidDistanceLabel = new(new TextDisplay("Avoid distance", font, Color.White, 1f));

        _grid = new Grid(4, 2, spacing, margin);
        _grid.AddComponent(maxSpeedLabel, 0, 0);
        _grid.AddComponent(maxSpeedInput, 0, 1);
        _grid.AddComponent(minSpeedLabel, 1, 0);
        _grid.AddComponent(minSpeedInput, 1, 1);
        _grid.AddComponent(flockDistanceLabel, 2, 0);
        _grid.AddComponent(flockDistanceInput, 2, 1);
        _grid.AddComponent(avoidDistanceLabel, 3, 0);
        _grid.AddComponent(avoidDistanceInput, 3, 1);

        inputManager.RegisterLeftClick(maxSpeedInput);
        inputManager.RegisterLeftClick(minSpeedInput);
        inputManager.RegisterLeftClick(flockDistanceInput);
        inputManager.RegisterLeftClick(avoidDistanceInput);
    }

    public override void FinalizeItem()
    {
        base.FinalizeItem();
        _grid.FinalizeGrid();
        Width = _grid.Width;
        Height = _grid.Height;
    }

    public override void FrameTick(IFrameTickManager frameTickManager)
    {
        base.FrameTick(frameTickManager);
        _grid.FrameTick(frameTickManager);
        _grid.UpdatePosition(Position);
    }

    public override void Draw(ISpriteBatchWrapper spriteBatch)
    {
        base.Draw(spriteBatch);
        _grid.Draw(spriteBatch);
    }
}
