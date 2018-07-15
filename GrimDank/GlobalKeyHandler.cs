using Microsoft.Xna.Framework.Input;

namespace GrimDank
{
    // Handles "global" keys that pretty much work everywhere. TODO: We probably should be handling InputStack.Add in constructors, because this leads
    // to it being very confusing what the stack order is (it becomes order of initialization).
    class GlobalKeyHandler : IInputHandler
    {
        public GlobalKeyHandler()
        {
            InputStack.Add(this);
        }

        public bool HandleKeyboard(KeyboardState state)
        {
            if (state.IsKeyDown(Keys.Escape))
            {
                GrimDank.Instance.Exit();
                return true;
            }

            return false;
        }
    }
}
