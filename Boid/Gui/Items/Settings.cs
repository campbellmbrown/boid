using Boid.Gui.Layout;
using Boid.Visual;

namespace Boid.Gui.Items;

public interface ISettings : IGuiItem
{
}

public class Settings : GuiItem, ISettings
{
    readonly IGrid _grid;

    public Settings(ILayerView layerView, GuiPlacement placement, IGrid grid)
        : base(layerView, placement)
    {
        _grid = grid;
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
