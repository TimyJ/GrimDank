using Microsoft.Xna.Framework.Input;
using GoRogue;

namespace GrimDank.MObjects.Components
{
    class PlayerAI : AIBase, IInputHandler
    {
        public bool _isTakingTurn;
        public bool IsTakingTurn
        {
            get => _isTakingTurn;
            set
            {
                if (_isTakingTurn != value)
                {
                    _isTakingTurn = value;

                    // Won't add duplicates because we had to actually change the value to get here.
                    if (!_isTakingTurn)
                        InputStack.Remove(this);
                    else
                        InputStack.Add(this);
                }


            }
        }

        public PlayerAI(Player parent)
            : base(parent) { }

        public bool HandleKeyboard(Keys key, ModifierState modifierState)
        {
            bool handledSomething = true;

            Direction dirToMove = Direction.NONE;
            Parent.CurrentMap.EnemyStatusToggle = false;

            switch (key)
            {
                case Keys.NumPad6:
                case Keys.L:
                    dirToMove = Direction.RIGHT;
                    break;
                case Keys.NumPad4:
                case Keys.H:
                    dirToMove = Direction.LEFT;
                    break;
                case Keys.NumPad8:
                case Keys.K:
                    dirToMove = Direction.UP;
                    break;
                case Keys.NumPad2:
                case Keys.J:
                    dirToMove = Direction.DOWN;
                    break;
                case Keys.T:
                    if (Parent.CurrentMap.Targeter == null)
                    {
                        Parent.CurrentMap.Targeter = new Targeting(c => MessageLog.Write($"Targeted {c}"));
                        // This should probably add itself.
                        InputStack.Add(Parent.CurrentMap.Targeter);
                    }
                    break;
                case Keys.LeftAlt:
                    Parent.CurrentMap.EnemyStatusToggle = true;
                    break;
                default:
                    handledSomething = false;
                    break;
            }

            if (dirToMove != Direction.NONE)
            {
                if (!Parent.MoveIn(dirToMove))
                {
                    MObject mobject = Parent.CurrentMap.Raycast(Parent.Position + dirToMove);
                    if (mobject is Creature mob) // Nice shorthand -- if mobjet is Creature, call it mob and let me work with it
                    {
                        mob.TakeDamage(10);
                        IsTakingTurn = false;
                    }
                }
                else // We moved so our turn is done.
                    IsTakingTurn = false;
            }

            return handledSomething;
        }

        
        public override void TakeTurn()
        {
            IsTakingTurn = true;
        }
    }
}
