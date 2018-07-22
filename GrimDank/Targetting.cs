using GoRogue;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GrimDank
{
    class Targetting : IInputHandler
    {
        public Coord TargetPos;
        private Func<Coord, bool> ActionOnSelection;
        private List<MObjects.Creature> potentialTargets;
        private int CurrentTarget;


        public Targetting(Coord pos, Func<Coord, bool> act)
        {
            TargetPos = pos;
            ActionOnSelection = act;
            potentialTargets = new List<MObjects.Creature>();
            foreach(Coord position in GrimDank.Instance.TestLevel.fov.CurrentFOV)
            {
                foreach(MObjects.Creature mob in GrimDank.Instance.TestLevel.GetLayer(Map.Layer.CREATURES).GetItems(position))
                {
                    if (mob != null)
                    {
                        potentialTargets.Add(mob);
                    }
                }
            }
            if (potentialTargets.Count > 1)
            {
                TargetPos = potentialTargets[1].Position;
                CurrentTarget = 0;
            }
        }

        public bool HandleKeyboard(KeyboardState state)
        {
            Direction dirToMove = Direction.NONE;
            bool handled = true;
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
                    case (int)Keys.Enter:
                        if (ActionOnSelection(TargetPos))
                        {
                            InputStack.Remove(this);
                            GrimDank.Instance.TestLevel.Targetter = null;
                        }
                        handled = true;
                        break;
                    case (int)Keys.Add:
                        if(CurrentTarget < potentialTargets.Count-1)
                        {
                            CurrentTarget += 1;
                            TargetPos = potentialTargets[CurrentTarget].Position;
                            break;
                        } else
                        {
                            CurrentTarget = 0;
                            TargetPos = potentialTargets[CurrentTarget].Position;
                            break;
                        }
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
