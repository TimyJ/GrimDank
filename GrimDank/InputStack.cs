using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrimDank
{
    static class InputStack
    {
        static private List<IInputHandler> _handlers = new List<IInputHandler>();
        static public IReadOnlyList<IInputHandler> Handlers { get => _handlers.AsReadOnly(); }

        private static readonly float INPUT_DELAY = 0.04f;
        private static readonly float INITIAL_INPUT_DELAY = 0.8f;
        private static float _timeSinceLastInput;
        private static bool somethingPressedInitial = false;
        private static bool somethingPressedSubsequent = false;

        private static Keys[] previousPressed = new Keys[0];

        static public void Add(IInputHandler handler)
        {
            // Debug check for derpsies
            Debug.Assert(!_handlers.Contains(handler));

            _handlers.Add(handler);
        }

        static public void Remove(IInputHandler handler) => _handlers.Remove(handler);

        static public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            // Loop through anything that has been released since last frame; we assume only one thing pressed at a time so this means whatever was pressed was
            // released. This really, really will NOT support modifiers, like at all, though.
            foreach (var key in previousPressed.Except(keyboardState.GetPressedKeys()))
            {
                somethingPressedInitial = somethingPressedSubsequent = false;
                _timeSinceLastInput = 0;
                break;
            }

            previousPressed = keyboardState.GetPressedKeys();

            if (!somethingPressedInitial || _timeSinceLastInput >= (somethingPressedSubsequent ? INPUT_DELAY : INITIAL_INPUT_DELAY))
            {
                foreach (var handler in _handlers)
                    if (handler.HandleKeyboard(keyboardState))
                    {
                        _timeSinceLastInput = 0;
                        if (somethingPressedInitial)
                            somethingPressedSubsequent = true;
                        else
                            somethingPressedInitial = true;
                        break;
                    }
            }
           else
                _timeSinceLastInput += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
