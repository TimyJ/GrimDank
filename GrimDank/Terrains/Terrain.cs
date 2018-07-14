using Microsoft.Xna.Framework;

namespace GrimDank.Terrains
{
    // Partial class -- Terrain_Types.cs contains the static instances for each terrain type.  To enforce that they are static instances,
    // constructor is private so only Terrain can create instances -- the intent is to force all instances to be made in Terrain_Types.cs
    // so that we don't accidentally create instances somewhere.
    partial class Terrain
    {
        public bool IsWalkable { get; set; }
        public bool IsTransparent { get; set; }
        public Color BackgroundColor { get; set; }
        public char Glyph { get; set; }

        // Private so only terrain class can create them, since they are static at the moment.
        private Terrain(bool isWalkable, bool isTransparent, Color backgroundColor, char glyph)
        {
            IsWalkable = isWalkable;
            IsTransparent = isTransparent;
            BackgroundColor = backgroundColor;
            Glyph = glyph;

        }
    }
}
