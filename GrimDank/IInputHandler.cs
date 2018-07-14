using Microsoft.Xna.Framework.Input;


namespace GrimDank
{
    // Implement to handle keyboard input.  Any implementer needs to call InputStack.Add()
    // before they can process input.
    interface IInputHandler
    {
        // Called by InputStack if this object is added to its Handlers.  It respects input delay
        // so this function will only be called if input delay (controlled by InputStack) allows it.
        bool HandleKeyboard(KeyboardState state);
    }
}
