using GoRogue;
using System;

namespace GrimDank.MObjects
{
    public class MovedArgs : EventArgs
    {
        public Coord OldPosition { get; private set; }
        public Coord NewPosition { get; private set; }

        public MovedArgs(Coord oldPosition, Coord newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }
    }

    class MObject : IHasID
    {
        // This will pretty much be unsat later as far as serialization goes but easy enough to sort out then
        private static readonly IDGenerator _idGen = new IDGenerator();

        public uint ID { get; private set; }

        public Map CurrentMap { get; private set; }

        // Making this publicly settable would be kinda tough (though doable) because Map would have to move things around when its set.
        // If we need to we can but otherwise meh.
        public Map.Layer Layer { get; private set; }
        public bool IsWalkable { get; set; }
        public bool IsTransparent { get; set; }

        private Coord _position;
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

        public event EventHandler<MovedArgs> Moved;

        // Can add parameters just here for ID stuff.
        public MObject(Map.Layer layer, Coord position, bool isWalkable = false, bool isTransparent = true)
        {
            ID = _idGen.UseID();

            Layer = layer;
            Position = position;
            IsWalkable = isWalkable;
            IsTransparent = isTransparent;

            Moved = null;
        }

        public bool MoveIn(Direction direction)
        {
            var oldPos = _position;
            Position += direction;

            return !(_position == oldPos);
        }

        // Do NOT call this unless you are Map.Add/Remove functions.  Bad!
        internal void _onMapChanged(Map map) => CurrentMap = map;
    }
}
