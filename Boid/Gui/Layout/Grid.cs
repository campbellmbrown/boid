using System;
using System.ComponentModel;
using System.Linq;
using Boid.Visual;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Boid.Gui.Layout;

public interface IGrid : IGuiElement, IVisualRelative, IFrameTickable
{
    void AddComponent(IGuiComponent component, int row, int col);
    void FinalizeGrid();
}

public class Grid : IGrid
{
    readonly int _spacing;
    readonly int _margin;
    readonly IGuiComponent[,] _components;
    readonly int[] _maxColWidths;
    readonly int[] _maxRowHeights;

    public int Width { get; private set; }
    public int Height { get; private set; }

    public Grid(int numRows, int numCols, int spacing, int margin)
    {
        _components = new IGuiComponent[numRows, numCols];
        _maxColWidths = new int[numCols];
        _maxRowHeights = new int[numRows];
        _spacing = spacing;
        _margin = margin;
    }

    public void AddComponent(IGuiComponent component, int row, int col)
    {
        _components[row, col] = component;
    }

    public void FinalizeGrid()
    {
        int numRows = _components.GetLength(0);
        int numCols = numRows > 0 ? _components.GetLength(1) : 0;

        for (int rowIdx = 0; rowIdx < numRows; rowIdx++)
        {
            for (int colIdx = 0; colIdx < numCols; colIdx++)
            {
                var component = _components[rowIdx, colIdx];
                if (component != null)
                {
                    _maxColWidths[colIdx] = Math.Max(_maxColWidths[colIdx], component.Width);
                    _maxRowHeights[rowIdx] = Math.Max(_maxRowHeights[rowIdx], component.Height);
                }
            }
        }

        Width = _maxColWidths.Sum() + (_spacing * (numCols - 1)) + (2 * _margin);
        Height = _maxRowHeights.Sum() + (_spacing * (numRows - 1)) + (2 * _margin);

        for (int rowIdx = 0; rowIdx < numRows; rowIdx++)
        {
            for (int colIdx = 0; colIdx < numCols; colIdx++)
            {
                var component = _components[rowIdx, colIdx];
                component?.FinalizeComponent(_maxColWidths[colIdx], _maxRowHeights[rowIdx]);
            }
        }
    }

    public void FrameTick(IFrameTickManager frameTickManager)
    {
        for (int rowIdx = 0; rowIdx < _components.GetLength(0); rowIdx++)
        {
            for (int colIdx = 0; colIdx < _components.GetLength(1); colIdx++)
            {
                var component = _components[rowIdx, colIdx];
                component?.FrameTick(frameTickManager);
            }
        }
    }

    public void UpdatePosition(Vector2 position)
    {
        int heightOffset = _margin;
        for (int rowIdx = 0; rowIdx < _components.GetLength(0); rowIdx++)
        {
            int widthOffset = _margin;
            for (int colIdx = 0; colIdx < _components.GetLength(1); colIdx++)
            {
                var component = _components[rowIdx, colIdx];
                component?.UpdatePosition(position + new Vector2(widthOffset, heightOffset));
                widthOffset += _maxColWidths[colIdx];
                widthOffset += _spacing;
            }
            heightOffset += _maxRowHeights[rowIdx];
            heightOffset += _spacing;
        }
    }

    public void Draw(ISpriteBatchWrapper spriteBatch)
    {
        for (int rowIdx = 0; rowIdx < _components.GetLength(0); rowIdx++)
        {
            for (int colIdx = 0; colIdx < _components.GetLength(1); colIdx++)
            {
                var component = _components[rowIdx, colIdx];
                component?.Draw(spriteBatch);
            }
        }
    }
}
