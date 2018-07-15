using Microsoft.Xna.Framework.Graphics;
using XNARect = Microsoft.Xna.Framework.Rectangle;
using GoRogue;

namespace GrimDank
{
    // Encompasses a Texture2D font, along with the size of the font sprites and number of columns
    class TextureFont
    {
        public Texture2D Texture { get; private set; }
        public int FontSize { get; private set; }
        public int NumColumns { get; private set; }

        public TextureFont(Texture2D texture, int fontSize, int numColumns)
        {
            Texture = texture;
            FontSize = fontSize;
            NumColumns = numColumns;
        }

        // Gets the rectangle on the texture representing the given char.
        public XNARect GlyphRect(char ch)
        {
            Coord pos = Coord.ToCoord(ch, NumColumns);
            return new XNARect(pos.X * FontSize, pos.Y * FontSize, FontSize, FontSize);
        }
    }
}
