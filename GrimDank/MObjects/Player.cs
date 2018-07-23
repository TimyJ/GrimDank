using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoRogue;
using GrimDank.MObjects.Components;

namespace GrimDank.MObjects
{
    class Player : Creature
    {
        private int _fovRange;
        public int FOVRange
        {
            get => _fovRange;
            set
            {
                if (value != _fovRange)
                {
                    _fovRange = value;
                    CurrentMap?.RecalculateFOV(Position, _fovRange);
                }
            }
        }

        public Player(Coord position)
            : base(position, 100, 10, "1d8", 0)
        {
            _fovRange = 23;

            glyph = '@';
            AIComponent = new PlayerAI(this);

            Moved += OnMoved;
        }

        private void OnMoved(object s, MovedArgs e)
        {
            // Kinda bad but here we assume if we have a map then that is the active map.
            if (CurrentMap != null)
            {
                CurrentMap?.RecalculateFOV(e.NewPosition, _fovRange);
                GrimDank.Instance.MapRenderer.Camera.Area = GrimDank.Instance.MapRenderer.Camera.Area.CenterOn(Position);
            }
        }
    }
}
