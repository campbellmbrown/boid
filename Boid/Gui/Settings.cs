using Boid.Gui.Layout;
using Boid.Visual;

namespace Boid.Gui;

public interface ISettings : IGuiItem
{
}

public class Settings : GuiItem, ISettings
{
    readonly IVerticalStack _stack;

    public Settings(ILayerView layerView, IVerticalStack verticalStack, GuiPlacement placement)
        : base(layerView)
    {
        _stack = verticalStack;
        Placement = placement;
    }

    public override void FinalizeItem()
    {
        base.FinalizeItem();
        _stack.FinalizeStack();
        Width = _stack.Width;
        Height = _stack.Height;
    }

    public override void FrameTick(IFrameTickManager frameTickManager)
    {
        _stack.UpdatePosition(Position);
    }

    public override void Draw(ISpriteBatchWrapper spriteBatch)
    {
        base.Draw(spriteBatch);
        _stack.Draw(spriteBatch);
    }
}
