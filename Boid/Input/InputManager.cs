using System.Collections.Generic;
using System.Linq;
using Boid.Visual;
using CreepyCrawler.Input;
using Microsoft.Xna.Framework.Input;

namespace Boid.Input;

public enum ClickState
{
    None,
    Hovered,
    Clicked,
}

public interface IInputManager : IFrameTickable
{
    void RegisterLeftClick(ILeftClickable leftClick);
}

record HeldKey : IFrameTickable
{
    const float _continuousModeThreshold = 0.5f;
    const float _continuousModeInterKeyDelay = 0.05f;

    public Keys Key { get; init; }
    bool _ready = true;
    public bool Ready
    {
        get
        {
            bool ready = _ready;
            _ready = false;
            return ready;
        }
    }

    bool _continuousMode = false;
    float _timer = 0f;

    public void FrameTick(IFrameTickManager frameTickManager)
    {
        _timer += frameTickManager.TimeDiffSec;
        if (_continuousMode)
        {
            if (_timer >= _continuousModeInterKeyDelay)
            {
                _ready = true;
                _timer -= _continuousModeInterKeyDelay;
            }
        }
        else
        {
            if (_timer >= _continuousModeThreshold)
            {
               _continuousMode = true;
               _ready = true;
               _timer = 0f;
            }
        }
    }
}

public class InputManager : IInputManager
{
    readonly ILayerView _layerView;
    readonly IMouseWrapper _mouseWrapper;
    readonly IKeyboardWrapper _keyboardWrapper;

    readonly List<ILeftClickable> _leftClicks = new();
    readonly List<HeldKey> _heldKeys = new();

    ButtonState _previousLeftButtonState = ButtonState.Released;

    ILeftClickable? _leftClicked = null;

    public InputManager(ILayerView layerView, IMouseWrapper mouseWrapper, IKeyboardWrapper keyboardWrapper)
    {
        _layerView = layerView;
        _mouseWrapper = mouseWrapper;
        _keyboardWrapper = keyboardWrapper;
    }

    public void RegisterLeftClick(ILeftClickable leftClick) => _leftClicks.Add(leftClick);

    public void FrameTick(IFrameTickManager frameTickManager)
    {
        var mouseState = _mouseWrapper.MouseState;
        var leftButtonState = mouseState.LeftButton;
        HandleLeftClick(leftButtonState);

        var pressedKeys = _keyboardWrapper.GetPressedKeys();
        HandleKeyboardInput(frameTickManager, pressedKeys);
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
            if ((_previousLeftButtonState == ButtonState.Pressed) && (_leftClicked != null) && _leftClicked.LeftClickArea.Contains(_layerView.MousePosition))
            {
                ResetAllFocus();
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

    void HandleKeyboardInput(IFrameTickManager frameTickManager, Keys[] keys)
    {
        foreach (var key in keys)
        {
            var keyAlreadyHeld = _heldKeys.Find(k => k.Key == key);
            if (keyAlreadyHeld == null)
            {
                _heldKeys.Add(new HeldKey { Key = key });
            }
        }
        _heldKeys.RemoveAll(heldKey => !keys.Any(k => k == heldKey.Key));

        foreach (var key in _heldKeys)
        {
            key.FrameTick(frameTickManager);
        }

        foreach (var leftClick in _leftClicks)
        {
            if (leftClick.Focused)
            {
                foreach (var heldKey in _heldKeys)
                {
                    if (heldKey.Ready)
                    {
                        leftClick.KeyPressed(heldKey.Key);
                    }
                }
            }
        }
    }

    void ResetAllFocus()
    {
        foreach (var leftClick in _leftClicks)
        {
            leftClick.Focused = false;
        }
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
