using Microsoft.Xna.Framework;

namespace GrimDank.Terrains
{
    // Here we define instances for all terrain types?  I think.
    partial class Terrain
    {
        public static readonly Terrain WALL = new Terrain(false, false, Color.Black, '#');
        public static readonly Terrain FLOOR = new Terrain(true, true, Color.Black, '.');
    }
}
