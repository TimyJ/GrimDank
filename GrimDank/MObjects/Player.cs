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
        public Player(Coord position)
            : base(position, 100, 10, "1d8", 0)
        {
            glyph = '@';
            AIComponent = new PlayerAI(this);
        }
    }
}
