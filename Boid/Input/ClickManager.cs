using System.Collections.Generic;
using Boid.Visual;
using Microsoft.Xna.Framework.Input;

namespace Boid.Input;

public enum ClickState
{
    None,
    Hovered,
    Clicked,
}

public interface IClickManager : IFrameTickable
{
    void RegisterLeftClick(ILeftClickable leftClick);
}

public class ClickManager : IClickManager
{
    readonly ILayerView _layerView;
    readonly IMouseWrapper _mouseWrapper;

    readonly List<ILeftClickable> _leftClicks = new();

    ButtonState _previousLeftButtonState = ButtonState.Released;

    ILeftClickable? _leftClicked = null;

    public ClickManager(ILayerView layerView, IMouseWrapper mouseWrapper)
    {
        _layerView = layerView;
        _mouseWrapper = mouseWrapper;
    }

    public void RegisterLeftClick(ILeftClickable leftClick) => _leftClicks.Add(leftClick);

    public void FrameTick(IFrameTickManager frameTickManager)
    {
        var mouseState = _mouseWrapper.MouseState;
        var leftButtonState = mouseState.LeftButton;

        HandleLeftClick(leftButtonState);
    }

    void HandleLeftClick(ButtonState leftButtonState)
    {
        if (leftButtonState == ButtonState.Pressed)
        {
            if (_previousLeftButtonState == ButtonState.Released)
            {
                // Clicked for the first time
                ResetAllLeftClicks();
                StoreLeftClicked();
            }

            if (_leftClicked != null)
            {
                _leftClicked.ChangeState(
                    _leftClicked.LeftClickArea.Contains(_layerView.MousePosition) ? ClickState.Clicked : ClickState.None
                );
            }
        }
        else
        {
            // Check for release
            if ((_previousLeftButtonState == ButtonState.Pressed) && (_leftClicked != null) && (_leftClicked.LeftClickArea.Contains(_layerView.MousePosition)))
            {
                _leftClicked.LeftClickAction();
            }

            _leftClicked = null;
            foreach (var leftClick in _leftClicks)
            {
                leftClick.ChangeState(
                    leftClick.LeftClickArea.Contains(_layerView.MousePosition) ? ClickState.Hovered : ClickState.None
                );
            }
        }

        _previousLeftButtonState = leftButtonState;
    }

    void ResetAllLeftClicks()
    {
        foreach (var leftClick in _leftClicks)
        {
            leftClick.ChangeState(ClickState.None);
        }
    }

    void StoreLeftClicked()
    {
        foreach (var leftClick in _leftClicks)
        {
            if (leftClick.LeftClickArea.Contains(_layerView.MousePosition))
            {
                _leftClicked = leftClick;
                return;
            }
            _leftClicked = null; // Nothing was clicked.
        }
    }
}
