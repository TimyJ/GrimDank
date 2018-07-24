using GoRogue;
using Microsoft.Xna.Framework.Input;

namespace GrimDank
{
    // Temporary marshalling place for repetitive code like movement-style keys that are used in many
    // places.  Could potentially be expanded/refactored later to include key-bindings and actions-checks.
    static class Controls
    {
        static public Direction Move(Keys key)
        {
            var dirToMove = Direction.NONE;

            switch (key)
            {
                case Keys.NumPad8:
                case Keys.K:
                    dirToMove = Direction.UP;
                    break;
                case Keys.NumPad9:
                case Keys.U:
                    dirToMove = Direction.UP_RIGHT;
                    break;
                case Keys.NumPad6:
                case Keys.L:
                    dirToMove = Direction.RIGHT;
                    break;
                case Keys.NumPad3:
                case Keys.N:
                    dirToMove = Direction.DOWN_RIGHT;
                    break;
                case Keys.NumPad2:
                case Keys.J:
                    dirToMove = Direction.DOWN;
                    break;
                case Keys.NumPad1:
                case Keys.B:
                    dirToMove = Direction.DOWN_LEFT;
                    break;
                case Keys.NumPad4:
                case Keys.H:
                    dirToMove = Direction.LEFT;
                    break;
                case Keys.NumPad7:
                case Keys.Y:
                    dirToMove = Direction.UP_LEFT;
                    break;
            }

            return dirToMove;
        }
    }
}
