using GoRogue;
using GoRogue.MapViews;

namespace GrimDank
{
    class ResistanceProvider : IMapView<double>
    {
        public Map Map { get; private set; }

        public ResistanceProvider(Map mapToUse) { Map = mapToUse; }

        public double this[Coord pos] { get { return Map.IsTransparent(pos) ?  0 : 1; } }

        public double this[int x, int y] { get { return Map.IsTransparent(Coord.Get(x, y)) ? 0 : 1; } }

        int IMapView<double>.Height => Map.Height;

        int IMapView<double>.Width => Map.Width;
    }
}
