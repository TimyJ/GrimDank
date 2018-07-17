using GoRogue;
using Microsoft.Xna.Framework.Input;

namespace GrimDank
{
    public delegate void Action();

    class Targetting : IInputHandler
    {
        public Coord TargetPos;
        public Action ActionOnSelection;

        public Targetting(Coord pos, Action act)
        {
            TargetPos = pos;
            ActionOnSelection = act;
        }

        public bool HandleKeyboard(KeyboardState state)
        {
            Direction dirToMove = Direction.NONE;
            bool handled = false;
            foreach (int key in state.GetPressedKeys())
            { 
                switch (key)
                {
                    case (int)Keys.NumPad6:
                        dirToMove = Direction.RIGHT;
                        break;
                    case (int)Keys.NumPad4:
                        dirToMove = Direction.LEFT;
                        break;
                    case (int)Keys.NumPad8:
                        dirToMove = Direction.UP;
                        break;
                    case (int)Keys.NumPad2:
                        dirToMove = Direction.DOWN;
                        break;
                    case (int)Keys.Escape:
                        InputStack.Remove(this);
                        GrimDank.Instance.TestLevel.Targetter = null;
                        handled = true;
                        break;
                    default:
                        break;
                }
            }
            if(dirToMove != Direction.NONE)
            { 
                TargetPos += dirToMove;
                handled = true;
            }

            return handled;
        }
    }
}
