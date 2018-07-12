namespace GrimDank.Terrains
{
    // Here we define instances for all terrain types?  I think.
    partial class Terrain
    {
        static readonly Terrain WALL = new Terrain(false, false);
        static readonly Terrain FLOOR = new Terrain(true, true);
    }
}
