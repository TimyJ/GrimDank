using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rectangle = Microsoft.Xna.Framework.Rectangle;
using GoRogue;
using GoRogue.MapViews;

namespace GrimDank
{
    class MapRenderer
    {
        public Texture2D CurrentFont;
        public Map CurrentMap { get; private set; }
        public BoundedRectangle Camera { get; private set; }
        private static readonly int FONT_COLUMNS = 16;

        public MapRenderer(Texture2D font, Map map)
        {
            CurrentFont = font;
            CurrentMap = map;
            Coord screenCells = Coord.Get(GrimDank.WINDOW_WIDTH / GrimDank.FONT_SIZE, GrimDank.WINDOW_HEIGHT / GrimDank.FONT_SIZE);
            Camera = new BoundedRectangle(new GoRogue.Rectangle(0, 0, screenCells.X, screenCells.Y), CurrentMap.WalkabilityMap.Bounds());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawWithRaycasting(spriteBatch);
            //DrawWithIteration(spriteBatch);
        }

        public static Coord WorldToPixel(Coord worldCoords) => worldCoords * GrimDank.FONT_SIZE;

        public static Coord PixelToWorld(Coord pixelCoords) => pixelCoords / 12;

        public void UpdateCameraSize(int deltaWidth, int deltaHeight)
        {
            Camera.Area = Camera.Area.ChangeSize(deltaWidth, deltaHeight);
        }

        private void DrawWithRaycasting(SpriteBatch spriteBatch)
        {
            int offsetX = Camera.Area.X*12;
            int offsetY = Camera.Area.Y*12;
            foreach (var worldPos in Camera.Area.Positions())
            {
                Coord screenPos = WorldToPixel(worldPos - Camera.Area.Position);

                if (CurrentMap.fov[worldPos.X, worldPos.Y] != 0)
                {
                    spriteBatch.Draw(CurrentFont, destinationRectangle: CellRect(screenPos), sourceRectangle: GlyphRect((char)0),
                                     color: CurrentMap.GetTerrain(worldPos).BackgroundColor);

                    var mob = CurrentMap.Raycast(worldPos);
                    if (mob != null)
                    {
                        spriteBatch.Draw(CurrentFont, destinationRectangle: CellRect(screenPos), sourceRectangle: GlyphRect(mob.glyph),
                                         color: Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(CurrentFont, destinationRectangle: CellRect(screenPos), sourceRectangle: GlyphRect(CurrentMap.GetTerrain(worldPos).Glyph),
                                         color: Color.White);
                    }
                }
                else if (CurrentMap.GetExplored(worldPos))
                {
                    spriteBatch.Draw(CurrentFont, destinationRectangle: CellRect(screenPos), sourceRectangle: GlyphRect(CurrentMap.GetTerrain(worldPos).Glyph),
                                     color: Color.Gray);
                }
            }
        }

        [Obsolete] // This code is NOT kept up to date; if needed it could be brought up to speed
        private void DrawWithIteration(SpriteBatch spriteBatch)
        {
            foreach (var worldPos in Camera.Area.Positions())
            {
                Coord screenPos = WorldToPixel(worldPos - Camera.Area.Position);

                spriteBatch.Draw(CurrentFont, destinationRectangle: CellRect(screenPos), sourceRectangle: GlyphRect((char)0),
                                 color: CurrentMap.GetTerrain(worldPos).BackgroundColor);
                spriteBatch.Draw(CurrentFont, destinationRectangle: CellRect(screenPos), sourceRectangle: GlyphRect(CurrentMap.GetTerrain(worldPos).Glyph),
                                 color: Color.White);
            }

            foreach (var mob in CurrentMap.MObjects)
            {
                if (Camera.Area.Contains(mob.Position)) {
                    Coord screenPos = WorldToPixel(mob.Position - Camera.Area.Position);

                    spriteBatch.Draw(CurrentFont, destinationRectangle: CellRect(screenPos), sourceRectangle: GlyphRect((char)0),
                                     color: CurrentMap.GetTerrain(mob.Position).BackgroundColor);
                    spriteBatch.Draw(CurrentFont, destinationRectangle: CellRect(screenPos), sourceRectangle: GlyphRect(mob.glyph),
                                     color: Color.White);
                }
            } 
                
        }

        // Gets rectangle from texture representing given character.
        private static rectangle GlyphRect(char character)
        {
            Coord pos = Coord.ToCoord(character, FONT_COLUMNS);
            return new rectangle(pos.X * GrimDank.FONT_SIZE, pos.Y * GrimDank.FONT_SIZE, GrimDank.FONT_SIZE, GrimDank.FONT_SIZE);
        }

        // Gets the rectangle representing the cell area for the cell located at the given screen coords.
        private static rectangle CellRect(Coord screenPos) => new rectangle(screenPos.X, screenPos.Y, GrimDank.FONT_SIZE, GrimDank.FONT_SIZE);
    }
}
