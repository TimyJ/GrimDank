using GoRogue;
using Microsoft.Xna.Framework;
using XNARect = Microsoft.Xna.Framework.Rectangle;

namespace GrimDank
{
    // Extensions for various MonoGame and GoRogue classes pertaining to integrating the two libraries, particularly
    // where they both have types representing different things.
    static class IntegrationExtensions
    {
        // Convert between point representations
        public static Coord ToCoord(this Point p) => Coord.Get(p.X, p.Y);
        public static Point ToPoint(this Coord c) => new Point(c.X, c.Y);

        // Convert between rectangle representations
        public static GoRogue.Rectangle ToGoRogueRect(this XNARect r) => new GoRogue.Rectangle(r.X, r.Y, r.Width, r.Height);
        public static XNARect ToXNARect(this GoRogue.Rectangle r) => new XNARect(r.X, r.Y, r.Width, r.Height);
    }
}
