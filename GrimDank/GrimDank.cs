using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using rectangle = Microsoft.Xna.Framework.Rectangle;

using GoRogue;

namespace GrimDank
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 

    public class Cell
    {
        public Color foreground;
        public Color background;
        public char glyph;

        public Cell()
        {
            foreground = Color.White;
            background = Color.Black;
            glyph = '.';
        }
    }

    public class GrimDank : Game
    {
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Texture2D font12x12;
        private static int testMapWidth = 50;
        private static int testMapHeight = 50;
        private static int fontColums = 16;
        private Map testLevel;
        
        public GrimDank()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            testLevel = new Map(20, 20);
            var player = new MObjects.MObject(Map.Layer.CREATURES, Coord.Get(10, 10), false, false);
            player.glyph = '@';
            testLevel.Add(player);
            for(var x = 0; x < testMapWidth; ++x)
            {
                for (var y = 0; y < testMapHeight; y++)
                {
                    testLevel.Add(new MObjects.MObject(Map.Layer.TERRAIN, Coord.Get(x, y), true));

                }
            }

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //load font from the content manager
            font12x12 = Content.Load<Texture2D>("font12x12");
            MessageLog.Write("Font Loaded");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //test drawing
            spriteBatch.Begin();
            for(int i=0; i < testMapHeight*testMapWidth; ++i)
            {
                Coord pos = Coord.ToCoord(i, testMapWidth);

                //spriteBatch.Draw(font12x12, destinationRectangle: new rectangle(pos.X * 12, pos.Y * 12, 12, 12), sourceRectangle: new rectangle(0, 0, 12, 12), color: testing[i].background);
                //spriteBatch.Draw(font12x12, destinationRectangle: new rectangle(pos.X * 12, pos.Y * 12, 12, 12), sourceRectangle: GlyphRect('.'), color: testing[i].foreground);
                //spriteBatch.Draw(font12x12, destinationRectangle: new rectangle(i%testMapWidth*12, i/testMapWidth*12, 12, 12), sourceRectangle: GlyphRect(testing[i].glyph), color: testing[i].foreground);
                var mob = testLevel.Raycast(pos);
                if(mob != null)
                {
                    spriteBatch.Draw(font12x12, destinationRectangle: new rectangle(pos.X * 12, pos.Y * 12, 12, 12), sourceRectangle: GlyphRect(mob.glyph), color: Color.White);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private rectangle GlyphRect(char Character)
        {
            Coord pos = Coord.ToCoord(Character, fontColums);
            return new rectangle(pos.X*12, pos.Y*12, 12, 12);
        }
    }
}