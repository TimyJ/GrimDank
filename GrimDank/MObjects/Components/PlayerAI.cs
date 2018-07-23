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

        public bool HandleKeyboard(KeyboardState state)
        {
            bool handledSomething = false;

            Direction dirToMove = Direction.NONE;
            Parent.CurrentMap.EnemyStatusToggle = false;

            foreach (int key in state.GetPressedKeys())
            {
                handledSomething = true;
                switch (key)
                {
                    case (int)Keys.NumPad6:
                    case (int)Keys.L:
                        dirToMove = Direction.RIGHT;
                        break;
                    case (int)Keys.NumPad4:
                    case (int)Keys.H:
                        dirToMove = Direction.LEFT;
                        break;
                    case (int)Keys.NumPad8:
                    case (int)Keys.K:
                        dirToMove = Direction.UP;
                        break;
                    case (int)Keys.NumPad2:
                    case (int)Keys.J:
                        dirToMove = Direction.DOWN;
                        break;
                    case (int)Keys.T:
                        if (Parent.CurrentMap.Targetter == null)
                        {
                            Parent.CurrentMap.Targetter = new Targetting(c => MessageLog.Write($"Targeted {c}"), Parent.Position);
                            // This should probably add itself.
                            InputStack.Add(Parent.CurrentMap.Targetter);
                        }
                        break;
                    case (int)Keys.LeftAlt:
                        Parent.CurrentMap.EnemyStatusToggle = true;
                        break;
                    default:
                        handledSomething = false;
                        break;
                }

                if (handledSomething)
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
                // Prolly should hook be a thing that happens as an eventHandler to Player.Moved, where
                // it can simply call calculate for Player's current map.
                Parent.CurrentMap.fov.Calculate(Parent.Position, 23);

                foreach (var pos in Parent.CurrentMap.fov.NewlySeen)
                {
                    Parent.CurrentMap.SetExplored(true, pos);
                }
                // Ditto above -- hook player move event prolly preferable.
                GrimDank.Instance.MapRenderer.Camera.Area = GrimDank.Instance.MapRenderer.Camera.Area.CenterOn(GrimDank.Instance.Player.Position);
            }

            return handledSomething;
        }

        
        public override void TakeTurn()
        {
            IsTakingTurn = true;
        }
    }
}
