using Microsoft.Xna.Framework.Input;

namespace GrimDank
{
    // Handles "global" keys that pretty much work everywhere.
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
                Program.Game.Exit();
                return true;
            }

            return false;
        }
    }
}
