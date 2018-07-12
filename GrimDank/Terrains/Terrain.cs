namespace GrimDank.Terrains
{
    // Partial class -- Terrain_Types.cs contains the static instances for each terrain type.
    partial class Terrain
    {
        public bool IsWalkable { get; set; }
        public bool IsTransparent { get; set; }

        // Private so only terrain class can create them, since they are static at the moment.
        private Terrain(bool isWalkable = true, bool isTransparent = true)
        {
            IsWalkable = isWalkable;
            IsTransparent = isTransparent;
        }
    }
}
