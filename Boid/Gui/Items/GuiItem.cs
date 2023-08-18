using System;
using Boid.Visual;
using Microsoft.Xna.Framework;

namespace Boid.Gui.Items;

public interface IGuiItem : IGuiElement, IFrameTickable, IVisual
{
    void FinalizeItem();
}

public abstract class GuiItem : IGuiItem
{
    readonly ILayerView _layerView;
    readonly GuiPlacement _placement;
    bool _finalized = false;

    protected GuiItem(ILayerView layerView, GuiPlacement placement)
    {
        _layerView = layerView;
        _placement = placement;
    }

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    protected Vector2 Position => _layerView.Origin + _placement switch
    {
        GuiPlacement.TopLeft => Vector2.Zero,
        GuiPlacement.TopMiddle => new Vector2((_layerView.Size.X - Width) / 2f, 0f),
        GuiPlacement.TopRight => new Vector2(_layerView.Size.X - Width, 0f),
        GuiPlacement.CenterLeft => new Vector2(0, (_layerView.Size.Y - Height) / 2f),
        GuiPlacement.Center => new Vector2(_layerView.Size.X - Width, _layerView.Size.Y - Height) / 2f,
        GuiPlacement.CenterRight => new Vector2(_layerView.Size.X - Width, (_layerView.Size.Y - Height) / 2f),
        GuiPlacement.BottomLeft => new Vector2(0, _layerView.Size.Y - Height),
        GuiPlacement.BottomMiddle => new Vector2((_layerView.Size.X - Width) / 2f, _layerView.Size.Y - Height),
        GuiPlacement.BottomRight => new Vector2(_layerView.Size.X - Width, _layerView.Size.Y - Height),
        _ => throw new ArgumentException("Grid placement not supported."),
    };

    public virtual void FinalizeItem()
    {
        if (_finalized)
        {
            throw new InvalidOperationException("Attempted to finalize GUI item when already finalized.");
        }
        _finalized = true;
    }

    public virtual void FrameTick(IFrameTickManager frameTickManager)
    {
        if (!_finalized)
        {
            throw new InvalidOperationException("Attempted to run GUI item before finalizing.");
        }
    }

    public virtual void Draw(ISpriteBatchWrapper spriteBatch)
    {
        if (!_finalized)
        {
            throw new InvalidOperationException("Attempted to draw GUI item before finalizing.");
        }
    }
}
