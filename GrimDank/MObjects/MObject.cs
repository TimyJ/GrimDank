using GoRogue;
using System;

namespace GrimDank.MObjects
{
    // Arguments for Moved events.
    public class MovedArgs : EventArgs
    {
        public MovedArgs(Coord oldPosition, Coord newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        public Coord NewPosition { get; private set; }
        public Coord OldPosition { get; private set; }
    }

    internal class MObject : IHasID
    {
        // This will pretty much be unsat later as far as serialization goes but easy enough to sort
        // out then
        private static readonly IDGenerator _idGen = new IDGenerator();

        private Coord _position;

        public MObject(Map.Layer layer, Coord position, bool isWalkable = false, bool isTransparent = true)
        {
            ID = _idGen.UseID();

            Layer = layer;
            Position = position;
            IsWalkable = isWalkable;
            IsTransparent = isTransparent;
            backgroundColor = Microsoft.Xna.Framework.Color.Black;
            glyph = '.';

            Moved = null;
        }

        // Happens whenever something moves (after its position has been updated).
        public event EventHandler<MovedArgs> Moved;

        // Automatically kept up to date as Map.Add and Map.Remove functions are called.
        public Map CurrentMap { get; private set; }

        public uint ID { get; private set; }
        public bool IsTransparent { get; set; }

        public bool IsWalkable { get; set; }

        // Making this publicly changeable would be kinda tough (though doable) because Map would
        // have to move things around when its set. If we need to we can but otherwise too much work -_-.
        public Map.Layer Layer { get; private set; }

        public char glyph { get; set;  }
        public Microsoft.Xna.Framework.Color backgroundColor { get; set; }

        // This does collision detection and everything, feel free to set. MoveIn function is easier
        // for directions/verification though.
        public Coord Position
        {
            get => _position;

            set
            {
                if (_position != value)
                {
                    var oldPos = _position;

                    if (CurrentMap == null || !CurrentMap.Collides(this, value))
                    {
                        _position = value;
                        Moved?.Invoke(this, new MovedArgs(oldPos, value));
                    }
                }
            }
        }

        // Easy to move via directions.
        public bool MoveIn(Direction direction)
        {
            var oldPos = _position;
            Position += direction;

            return !(_position == oldPos);
        }

        // Do NOT call this unless you are Map.Add/Remove functions. Bad! Seriously, don't touch.
        internal void _onMapChanged(Map map) => CurrentMap = map;
    }
}