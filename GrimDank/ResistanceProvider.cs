using GoRogue;
using GoRogue.MapViews;

namespace GrimDank
{
    class ResistanceProvider : IMapView<double>
    {
        private Map map;
        public ResistanceProvider(Map mapToUse) { map = mapToUse; }

        public double this[Coord pos] { get { return map.IsTransparent(pos) ?  0 : 1; } }

        public double this[int x, int y] { get { return map.IsTransparent(Coord.Get(x, y)) ? 0 : 1; } }

        int IMapView<double>.Height => map.Height;

        int IMapView<double>.Width => map.Width;
    }
}
