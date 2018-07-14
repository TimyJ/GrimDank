using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;


namespace GrimDank
{
    // Implement to handle keyboard input.  Any implementer needs to call InputStack.Add()
    // before they can process input.
    interface IInputHandler
    {
        bool HandleKeyboard(KeyboardState state);
    }
}
