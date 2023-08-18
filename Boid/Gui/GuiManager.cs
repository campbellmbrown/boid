using System;
using System.Collections.Generic;
using Boid.Visual;

namespace Boid.Gui;

public interface IGuiManager : IVisual, IFrameTickable
{
    void AddItem(IGuiItem item);
    void FinalizeGui();
}

public class GuiManager : IGuiManager
{
    readonly List<IGuiItem> _items = new();

    bool _finalized = false;

    public void AddItem(IGuiItem item)
    {
        _items.Add(item);
    }

    public void FinalizeGui()
    {
        if (_finalized)
        {
            throw new InvalidOperationException("Attempted to finalize GUI manager when already finalized.");
        }
        foreach (var item in _items)
        {
            item.FinalizeItem();
        }
        _finalized = true;
    }

    public void Draw(ISpriteBatchWrapper spriteBatch)
    {
        if (!_finalized)
        {
            throw new InvalidOperationException("Attempted to draw GUI manager before finalizing.");
        }
        foreach (var item in _items)
        {
            item.Draw(spriteBatch);
        }
    }

    public void FrameTick(IFrameTickManager frameTickManager)
    {
        if (!_finalized)
        {
            throw new InvalidOperationException("Attempted to run GUI manager before finalizing.");
        }
        foreach (var item in _items)
        {
            item.FrameTick(frameTickManager);
        }
    }
}
