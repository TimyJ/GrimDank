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

        private static readonly float INPUT_DELAY = 0.1f;
        private static float _timeSinceLastInput;

        static public void Add(IInputHandler handler)
        {
            // Debug check for derpsies
            Debug.Assert(!_handlers.Contains(handler));

            _handlers.Add(handler);
        }

        static public void Remove(IInputHandler handler) => _handlers.Remove(handler);

        static public void Update(GameTime gameTime)
        {
            if (_timeSinceLastInput >= INPUT_DELAY)
            {
                var keyboardState = Keyboard.GetState();

                foreach (var handler in _handlers)
                    if (handler.HandleKeyboard(keyboardState))
                    {
                        _timeSinceLastInput = 0;
                        break;
                    }
            }
           else
                _timeSinceLastInput += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
