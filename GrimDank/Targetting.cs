using GoRogue;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
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
                var mob = (MObjects.Creature)GrimDank.Instance.TestLevel.GetLayer(Map.Layer.CREATURES).GetItems(position).SingleOrDefault();
                // Commented for debug, was testing something in linq because of my crash.  Either should work.
                //foreach(MObjects.Creature mob in GrimDank.Instance.TestLevel.GetLayer(Map.Layer.CREATURES).GetItems(position))
                //{
                
                    if (mob != null && mob != GrimDank.Instance.Player) // Temp index fix, just making sure it wasnt an input handling issue
                    {
                        potentialTargets.Add(mob);
                    }
                //}
            }
            if (potentialTargets.Count > 0)
            {
                TargetPos = potentialTargets[0].Position;
                CurrentTarget = 0;
            }
        }

        public bool HandleKeyboard(KeyboardState state)
        {
            Direction dirToMove = Direction.NONE;
            bool handled = false;
            
            foreach (int key in state.GetPressedKeys())
            {
                handled = true;
                
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
                        break;
                    case (int)Keys.Add:
                        CurrentTarget = MathHelpers.WrapAround(CurrentTarget + 1, potentialTargets.Count);
                        TargetPos = potentialTargets[CurrentTarget].Position;
                        break;

						// I changed this during moar debugging but hey its shorter :D
                        /*
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
                        */
                    case (int)Keys.Escape:
                        InputStack.Remove(this);
                        GrimDank.Instance.TestLevel.Targetter = null;
                        break;
                    default:
                        handled = false;
                        break;
                }
            }
            if (dirToMove != Direction.NONE)
            {
                TargetPos += dirToMove;
            }

            if (handled)
                MessageLog.Write("Returning true from targeter.");
            return handled;
        }
    }
}
