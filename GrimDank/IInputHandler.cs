using Microsoft.Xna.Framework.Input;


namespace GrimDank
{
    // Implement to handle keyboard input.  Any implementer needs to call InputStack.Add()
    // before they can process input.  This system cannot support checking any key independent of input delay --
    // this includes not supporting "push to activate" style keys, like press and hold alt to bring up a menu that
    // goes away when the key is released.
    interface IInputHandler
    {
        // Called by InputStack for each key pressed (if somebody higher in the stack hasn't already used).  It respects input
        // delay so this function will only be called if input delay (controlled by InputStack) allows it.
        bool HandleKeyboard(Keys key, ModifierState modifierState);
    }
}
