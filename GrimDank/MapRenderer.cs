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
        public Lerp2D targetInfoRect;

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
            DisplayEnemyStatus(spriteBatch);
            if( CurrentMap.Targetter != null )
            {
                DisplayTargetting(spriteBatch);
            }
        }

        public Coord WorldToPixel(Coord worldCoords) => worldCoords * _currentFont.FontSize;

        public Coord PixelToWorld(Coord pixelCoords) => pixelCoords / _currentFont.FontSize;

        public void UpdateCameraSize(int deltaWidth, int deltaHeight)
        {
            Coord center = Camera.Area.Center;
            // Yes chaining like this makes 2 rectangles (1 temp).  meh, don't care :D
            Camera.Area = Camera.Area.ChangeSize(deltaWidth, deltaHeight).CenterOn(center);

        }

        private void DisplayEnemyStatus(SpriteBatch spriteBatch)
        {
            if (CurrentMap.EnemyStatusToggle)
            {
                if (CurrentMap.Targetter == null)
                {
                    foreach (MObjects.Creature mob in CurrentMap.GetLayer(Map.Layer.CREATURES).Items)
                    {
                        Coord screenPos = WorldToPixel(mob.Position - Camera.Area.Position);
                        if (Camera.Area.Contains(mob.Position) && CurrentMap.fov[mob.Position] != 0)
                        {
                            spriteBatch.Draw(CurrentFont.Texture, new Vector2(screenPos.X + 12, screenPos.Y - 12), new XNARect(0, 0, 12, 12), Color.Black);
                            spriteBatch.Draw(CurrentFont.Texture, new XNARect(screenPos.X + 25, screenPos.Y - 24, 36, 12), new XNARect(0, 0, 12, 12), Color.Black);
                            spriteBatch.Draw(CurrentFont.Texture, new Vector2(screenPos.X + 12, screenPos.Y - 12), CurrentFont.GlyphRect('/').ToGoRogueRect().ToXNARect(), Color.LightGreen);
                            spriteBatch.DrawString(GrimDank.Instance.fpsFont, mob.CurrentEnergy.ToString(), new Vector2(screenPos.X + 24, screenPos.Y - 24), Color.Green);
                        }
                    }
                } else if (CurrentMap.GetSoleItem(Map.Layer.CREATURES, CurrentMap.Targetter.TargetPos) is MObjects.Creature mob)
                {
                    Coord screenPos = WorldToPixel(mob.Position - Camera.Area.Position);
                    spriteBatch.Draw(CurrentFont.Texture, new XNARect(screenPos.X + 40, screenPos.Y, 120, 80), new XNARect(0, 0, 12, 12), Color.Black);
                    spriteBatch.DrawString(GrimDank.Instance.fpsFont, mob.Name, new Vector2(screenPos.X + 40, screenPos.Y), Color.Green);
                }
            }
        }

        private void DisplayTargetting(SpriteBatch spriteBatch)
        {
            Coord screenPos = WorldToPixel(CurrentMap.Targetter.TargetPos - Camera.Area.Position);
            spriteBatch.Draw(GrimDank.Instance.reticle, new XNARect(screenPos.X - 15, screenPos.Y - 15, 40, 40), new XNARect(75, 75, 40, 40), Color.LightGreen);
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
