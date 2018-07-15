using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNARect = Microsoft.Xna.Framework.Rectangle;
using GoRogue;
using GoRogue.MapViews;

namespace GrimDank
{
    class MapRenderer
    {
        private TextureFont _currentFont;
        public TextureFont CurrentFont
        {
            get => _currentFont;
            set
            {
                _currentFont = value ?? throw new ArgumentNullException(nameof(CurrentFont));
                Coord screenCells = Coord.Get(GrimDank.Instance.WINDOW_WIDTH / _currentFont.FontSize, GrimDank.Instance.WINDOW_HEIGHT / _currentFont.FontSize);
                Camera = new BoundedRectangle(new GoRogue.Rectangle(0, 0, screenCells.X, screenCells.Y), CurrentMap.WalkabilityMap.Bounds());
            }
        }

        private Map _currentMap;
        public Map CurrentMap
        {
            get => _currentMap;
            set
            {
                _currentMap = value ?? throw new ArgumentNullException(nameof(CurrentMap));
                Camera = new BoundedRectangle(new GoRogue.Rectangle(0, 0, Camera.Area.Width, Camera.Area.Height), CurrentMap.WalkabilityMap.Bounds());
            }
        }
        public BoundedRectangle Camera { get; private set; }

        public MapRenderer(TextureFont font, Map map)
        {
            _currentMap = map ?? throw new ArgumentNullException(nameof(map));
            // Calls setter, which takes care of setting up camera
            CurrentFont = font ?? throw new ArgumentNullException(nameof(font));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawWithRaycasting(spriteBatch);
            //DrawWithIteration(spriteBatch);
        }

        public Coord WorldToPixel(Coord worldCoords) => worldCoords * _currentFont.FontSize;

        public Coord PixelToWorld(Coord pixelCoords) => pixelCoords / _currentFont.FontSize;

        public void UpdateCameraSize(int deltaWidth, int deltaHeight)
        {
            Coord center = Camera.Area.Center;
            // Yes chaining like this makes 2 rectangles (1 temp).  meh, don't care :D
            Camera.Area = Camera.Area.ChangeSize(deltaWidth, deltaHeight).CenterOn(center);

        }

        private void DrawWithRaycasting(SpriteBatch spriteBatch)
        {
            foreach (var worldPos in Camera.Area.Positions())
            {
                Coord screenPos = WorldToPixel(worldPos - Camera.Area.Position);

                if (CurrentMap.fov[worldPos.X, worldPos.Y] != 0)
                {
                    spriteBatch.Draw(CurrentFont.Texture, destinationRectangle: CellRect(screenPos), sourceRectangle: CurrentFont.GlyphRect((char)0),
                                     color: CurrentMap.GetTerrain(worldPos).BackgroundColor);

                    var mob = CurrentMap.Raycast(worldPos);
                    if (mob != null)
                    {
                        spriteBatch.Draw(CurrentFont.Texture, destinationRectangle: CellRect(screenPos), sourceRectangle: CurrentFont.GlyphRect(mob.glyph),
                                         color: Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(CurrentFont.Texture, destinationRectangle: CellRect(screenPos), sourceRectangle: CurrentFont.GlyphRect(CurrentMap.GetTerrain(worldPos).Glyph),
                                         color: Color.White);
                    }
                }
                else if (CurrentMap.GetExplored(worldPos))
                {
                    spriteBatch.Draw(CurrentFont.Texture, destinationRectangle: CellRect(screenPos), sourceRectangle: CurrentFont.GlyphRect(CurrentMap.GetTerrain(worldPos).Glyph),
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

                spriteBatch.Draw(CurrentFont.Texture, destinationRectangle: CellRect(screenPos), sourceRectangle: CurrentFont.GlyphRect((char)0),
                                 color: CurrentMap.GetTerrain(worldPos).BackgroundColor);
                spriteBatch.Draw(CurrentFont.Texture, destinationRectangle: CellRect(screenPos), sourceRectangle: CurrentFont.GlyphRect(CurrentMap.GetTerrain(worldPos).Glyph),
                                 color: Color.White);
            }

            foreach (var mob in CurrentMap.MObjects)
            {
                if (Camera.Area.Contains(mob.Position)) {
                    Coord screenPos = WorldToPixel(mob.Position - Camera.Area.Position);

                    spriteBatch.Draw(CurrentFont.Texture, destinationRectangle: CellRect(screenPos), sourceRectangle: CurrentFont.GlyphRect((char)0),
                                     color: CurrentMap.GetTerrain(mob.Position).BackgroundColor);
                    spriteBatch.Draw(CurrentFont.Texture, destinationRectangle: CellRect(screenPos), sourceRectangle: CurrentFont.GlyphRect(mob.glyph),
                                     color: Color.White);
                }
            } 
                
        }

        // Gets the rectangle representing the cell area for the cell located at the given screen coords.
        private XNARect CellRect(Coord screenPos) => new XNARect(screenPos.X, screenPos.Y, CurrentFont.FontSize, CurrentFont.FontSize);
    }
}
