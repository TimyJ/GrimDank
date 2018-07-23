using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using GrimDank.MObjects;
using GrimDank.MObjects.Components;

namespace GrimDank
{
    class TurnManager
    {
        public Map CurrentMap { get; private set; }
        private bool _finishedMonsterTurns; // Signals we just finished monster turns and player is up.


        public TurnManager(Map map)
        {
            CurrentMap = map;
            _finishedMonsterTurns = true;
        }

        public void Update(GameTime deltaTime)
        {
            // Not having the player must stop time!!!
            if (GrimDank.Instance.Player.CurrentMap == CurrentMap)
            {
                if (((PlayerAI)GrimDank.Instance.Player.AIComponent).IsTakingTurn || _finishedMonsterTurns)
                {
                    _finishedMonsterTurns = false; // Player is taking over
                    GrimDank.Instance.Player.AIComponent.TakeTurn(); // Starts accepting input if we aren't already
                }
                else
                {
                    // TODO: We clone the list so death doesn't screw things, but that means we NEED to death-check this, probably from the AI side, not here
                    foreach (Creature mob in CurrentMap.GetLayer(Map.Layer.CREATURES).Items.ToList())
                    {
                        if (mob.AIComponent == null || mob == GrimDank.Instance.Player)
                            continue;

                        mob.AIComponent.TakeTurn();
                    }

                    _finishedMonsterTurns = true;
                }
            }
        }
    }
}
