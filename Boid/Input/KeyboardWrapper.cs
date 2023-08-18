using Microsoft.Xna.Framework.Input;

namespace CreepyCrawler.Input;

public interface IKeyboardWrapper
{
    public Keys[] GetPressedKeys();
}

public class KeyboardWrapper : IKeyboardWrapper
{
    public Keys[] GetPressedKeys() => Keyboard.GetState().GetPressedKeys();
}
