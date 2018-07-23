using GoRogue;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Collections.Generic;

namespace GrimDank
{
    class Targeting : IInputHandler
    {
        public Coord TargetPos { get; private set; }
        private Func<Coord, bool> _targetValidator;
        private Action<Coord> _onTargetSelected;

        private List<Coord> _validTargets;
        private int _currentTargetIndex; // CANNOT assume this is in sync with TargetPos, because if we're free-floating the cursor it probably isnt.

        // TODO: Yes this allows the player to be targeted.  Should probably prevent that.
        private static readonly Func<Coord, bool> DEFAULT_TARGET_VALIDATOR = c =>
                                                                            (GrimDank.Instance.TestLevel.Raycast(c, m => m is MObjects.Creature) != null);

        // Default to just targeting creatures.
        public Targeting(Action<Coord> onTargetSelected, Coord targetPos = null, Func<Coord, bool> targetValidator = null)
        {
            _onTargetSelected = onTargetSelected;

            _targetValidator = targetValidator ?? DEFAULT_TARGET_VALIDATOR;
            // Create list of valid target locations in FOV, according to our selector
            _validTargets = new List<Coord>(GrimDank.Instance.TestLevel.CurrentFOV.Where(_targetValidator));

            if (_validTargets.Count == 0)
            {
                MessageLog.Write("No valid targets.");
                InputStack.Remove(this);
                GrimDank.Instance.TestLevel.Targeter = null;
            }

            if (targetPos != null) // -1 if the original given position isn't a valid target, otherwise we start at that point in the list.
            {
                _currentTargetIndex = _validTargets.FindIndex(c => c == targetPos);
                TargetPos = targetPos;
            }
            else // There are valid targets but we didn't select a position so default to first target in list
            {
                _currentTargetIndex = 0;
                TargetPos = _validTargets[_currentTargetIndex];
            }


        }

        public bool HandleKeyboard(Keys key, ModifierState modifierState)
        {
            Direction dirToMove = Direction.NONE;
            bool handled = true;
            
                
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
                case Keys.Enter: 
                    if (_targetValidator(TargetPos)) // CurrentPos is valid
                    {
                        _onTargetSelected(TargetPos);
                        InputStack.Remove(this);
                        GrimDank.Instance.TestLevel.Targeter = null;
                    }
                    break;
                case Keys.Add:
                    // This works even if we started at -1.
                    if (_validTargets.Count != 0)
                    {
                        _currentTargetIndex = MathHelpers.WrapAround(_currentTargetIndex + 1, _validTargets.Count);
                        TargetPos = _validTargets[_currentTargetIndex];
                    }
                    break;

                case Keys.Escape:
                    InputStack.Remove(this);
                    GrimDank.Instance.TestLevel.Targeter = null;
                    break;
                default:
                    handled = false;
                    break;
            }

            // Here we purposely do NOT reset the _currentTargetIndex to -1, to preserve the starting point in case the user presses + sometime again
            // in the future (we pick up where we left off)
            if (dirToMove != Direction.NONE)
            {

                TargetPos += dirToMove;
            }

            return handled;
        }
    }
}
