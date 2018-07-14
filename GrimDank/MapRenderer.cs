using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rectangle = Microsoft.Xna.Framework.Rectangle;
using GoRogue;

namespace GrimDank
{
    class MapRenderer
    {
        public Texture2D CurrentFont;
        public Map CurrentMap;
        public BoundedRectangle Camera;
        private static int fontColums = 16;

        public MapRenderer(Texture2D font, Map map)
        {
            CurrentFont = font;
            CurrentMap = map;
            Camera = new BoundedRectangle(new GoRogue.Rectangle(0, 0, 1280/12, 768/12), new GoRogue.Rectangle(0, 0, CurrentMap.Width-1, CurrentMap.Height-1));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawWithRaycasting(spriteBatch);
            //DrawWithIteration(spriteBatch);
        }

        public void UpdateCameraSize(int width, int height)
        {
            Camera.Area = Camera.Area.NewWithMaxCorner(Coord.Get(width + Camera.Area.MaxX, height + Camera.Area.MaxY));
        }

        private void DrawWithRaycasting(SpriteBatch spriteBatch)
        {
            int offsetX = Camera.Area.X*12;
            int offsetY = Camera.Area.Y*12;
            foreach (var pos in Camera.Area.Positions())
            {
                if (CurrentMap.fov[pos.X, pos.Y] != 0)
                {
                    spriteBatch.Draw(CurrentFont, destinationRectangle: new rectangle(pos.X * 12 -offsetX, pos.Y * 12-offsetY, 12, 12), sourceRectangle: new rectangle(0, 0, 12, 12), color: CurrentMap.GetTerrain(pos).BackgroundColor);

                    var mob = CurrentMap.Raycast(pos);
                    if (mob != null)
                    {
                        spriteBatch.Draw(CurrentFont, destinationRectangle: new rectangle(pos.X * 12-offsetX, pos.Y * 12-offsetY, 12, 12), sourceRectangle: GlyphRect(mob.glyph), color: Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(CurrentFont, destinationRectangle: new rectangle(pos.X * 12-offsetX, pos.Y * 12-offsetY, 12, 12), sourceRectangle: GlyphRect(CurrentMap.GetTerrain(pos).Glyph), color: Color.White);
                    }
                }
                else if (CurrentMap.GetExplored(pos))
                {
                    spriteBatch.Draw(CurrentFont, destinationRectangle: new rectangle(pos.X * 12-offsetX, pos.Y * 12-offsetY, 12, 12), sourceRectangle: GlyphRect(CurrentMap.GetTerrain(pos).Glyph), color: Color.Gray);
                }
            }
        }

        private void DrawWithIteration(SpriteBatch spriteBatch)
        {
            foreach (var pos in Camera.Area.Positions())
            {
                spriteBatch.Draw(CurrentFont, destinationRectangle: new rectangle(pos.X * 12, pos.Y * 12, 12, 12), sourceRectangle: new rectangle(0, 0, 12, 12), color: CurrentMap.GetTerrain(pos).BackgroundColor);
                spriteBatch.Draw(CurrentFont, destinationRectangle: new rectangle(pos.X * 12, pos.Y * 12, 12, 12), sourceRectangle: GlyphRect(CurrentMap.GetTerrain(pos).Glyph), color: Color.White);
            }

            foreach (var mob in CurrentMap.MObjects)
            {
                if (Camera.Area.Contains(mob.Position)) {
                    spriteBatch.Draw(CurrentFont, destinationRectangle: new rectangle(mob.Position.X * 12, mob.Position.Y * 12, 12, 12), sourceRectangle: new rectangle(0, 0, 12, 12), color: CurrentMap.GetTerrain(mob.Position).BackgroundColor);
                    spriteBatch.Draw(CurrentFont, destinationRectangle: new rectangle(mob.Position.X * 12, mob.Position.Y * 12, 12, 12), sourceRectangle: GlyphRect(mob.glyph), color: Color.White);
                }
            } 
                
        }

        private rectangle GlyphRect(char Character)
        {
            Coord pos = Coord.ToCoord(Character, fontColums);
            return new rectangle(pos.X * 12, pos.Y * 12, 12, 12);
        }
    }
}
