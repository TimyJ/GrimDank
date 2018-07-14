using GoRogue.MapViews;
using GoRogue;

namespace GrimDank
{
    // Provides walkability info from map.  Useful for AStar or other pathing stuff that cares
    class WalkabilityProvider : IMapView<bool>
    {
        public Map Map { get; private set; }
        public int Width { get => Map.Width; }
        public int Height { get => Map.Height; }

        public bool this[Coord pos] { get => Map.IsWalkable(pos); }
        public bool this[int x, int y] { get => Map.IsWalkable(Coord.Get(x, y)); }

        public WalkabilityProvider(Map mapToUse)
        {
            Map = mapToUse;
        }
    }
}
