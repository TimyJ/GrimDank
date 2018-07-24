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
            Parent.CurrentMap.EnemyStatusToggle = false;
            var dirToMove = Controls.Move(key);

            if (dirToMove == Direction.NONE)
            {
                switch (key)
                {
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
            }
            else if (!Parent.MoveIn(dirToMove))
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

            return handledSomething;
        }

        
        public override void TakeTurn()
        {
            IsTakingTurn = true;
        }
    }
}
