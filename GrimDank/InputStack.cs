using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrimDank
{
    struct ModifierState
    {
        public readonly bool Shift;
        public readonly bool Ctrl;
        public readonly bool Alt;

        public ModifierState(bool shift, bool ctrl, bool alt)
        {
            Shift = shift;
            Ctrl = ctrl;
            Alt = alt;
        }

    }

    static class InputStack
    {
        static readonly Keys[] MODIFIER_KEYS = { Keys.LeftShift , Keys.RightShift, Keys.LeftAlt, Keys.RightAlt, Keys.RightControl, Keys.LeftControl };

        private static readonly double INPUT_DELAY = 0.04f;
        private static readonly double INITIAL_INPUT_DELAY = 0.8f;

        static private List<IInputHandler> _handlers = new List<IInputHandler>();
        // Gets elements in stack order.  Casts to IEnumerable to ensure linq
        // reverse is called, rather than in-place reverse.
        static public IEnumerable<IInputHandler> Handlers
        { get => ((IEnumerable<IInputHandler>)new List<IInputHandler>(_handlers)).Reverse(); }

        private static double _timeSinceLastInput = 0.0;
        private static bool somethingPressedInitial = false;
        private static bool somethingPressedSubsequent = false;

        private static bool _inputLocked = false;
        public static bool InputLocked { get => _inputLocked; }

        private static double _timeToLockInput = 0.0;
        private static double _timeElapsedSinceInputLock = 0.0;


        private static Keys[] previousPressed = new Keys[0];

        static public void Add(IInputHandler handler)
        {
            // Debug check for derpsies
            Debug.Assert(!_handlers.Contains(handler));
            
            _handlers.Add(handler);
        }

        static public void Remove(IInputHandler handler) => _handlers.Remove(handler);

        static public void BlockInputFor(double timeInSeconds)
        {
            _inputLocked = true;
            _timeToLockInput = Math.Max(_timeToLockInput, timeInSeconds);

            _timeSinceLastInput = 0;
            somethingPressedInitial = somethingPressedSubsequent = false;
        }

        static public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            UpdateForReleasedKeys(keyboardState); 
            previousPressed = keyboardState.GetPressedKeys();

            UpdateInputLock(); 

            if (!_inputLocked && (!somethingPressedInitial || _timeSinceLastInput >= (somethingPressedSubsequent ? INPUT_DELAY : INITIAL_INPUT_DELAY)))
            {
                var modifierState = GetModifierState(keyboardState);

                var handlers = new List<IInputHandler>(_handlers);
                foreach (var key in keyboardState.GetPressedKeys())
                {
                    // Give each key a chance to handle the input.
                    foreach (var handler in ((IEnumerable<IInputHandler>)handlers).Reverse())
                    {
                        if (handler.HandleKeyboard(key, modifierState))
                        {
                            _timeSinceLastInput = 0;
                            if (somethingPressedInitial)
                                somethingPressedSubsequent = true;
                            else
                                somethingPressedInitial = true;
                            break;
                        }
                    }

                }
            }
            // Input locked, so just count input-locked timer
           else if (_inputLocked)
                _timeElapsedSinceInputLock += gameTime.ElapsedGameTime.TotalSeconds;
            else
                _timeSinceLastInput += gameTime.ElapsedGameTime.TotalSeconds;
        }

        static private ModifierState GetModifierState(KeyboardState keyState)
        {
            // TODO: Fancy bit-masking for this may be possible
            bool ctrl = false, shift = false, alt = false;

            if (keyState.IsKeyDown(Keys.LeftControl) || keyState.IsKeyDown(Keys.RightControl))
                ctrl = true;
            if (keyState.IsKeyDown(Keys.LeftShift) || keyState.IsKeyDown(Keys.RightShift))
                shift = true;
            if (keyState.IsKeyDown(Keys.LeftAlt) || keyState.IsKeyDown(Keys.RightAlt))
                alt = true;

            return new ModifierState(shift, ctrl, alt);
        }

        static private void UpdateForReleasedKeys(KeyboardState keyboardState)
        {
            // Loop through anything that has been released since last frame; we assume only one thing pressed at a time so this means whatever was pressed was
            // released. This really, really will NOT support modifiers, like at all, though.
            foreach (var key in previousPressed.Except(keyboardState.GetPressedKeys()))
            {
                somethingPressedInitial = somethingPressedSubsequent = false;
                _timeSinceLastInput = 0;
                break;
            }
        }

        static private void UpdateInputLock()
        {
            if (_inputLocked && _timeElapsedSinceInputLock >= _timeToLockInput)
            {
                _inputLocked = false;
                _timeElapsedSinceInputLock = _timeToLockInput = 0.0;
            }
        }
    }
}
