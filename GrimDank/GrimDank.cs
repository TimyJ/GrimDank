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
        private Cell[] testing;
        private static int testMapWidth = 50;
        private static int testMapHeight = 50;
        private static int fontColums = 16;
        
        public GrimDank()
        {
            // GoRogue test.
            System.Console.WriteLine(Coord.Get(1, 2));

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            testing = new Cell[testMapWidth * testMapHeight];
            for(int i = 0; i < testing.Length; ++i)
            {
                testing[i] = new Cell();
            }

            //check setting cells;
            testing[30].glyph = '@';
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
                spriteBatch.Draw(font12x12, destinationRectangle: new rectangle(i%testMapWidth*12, i/testMapWidth*12, 12, 12), sourceRectangle: new rectangle(0, 0, 12, 12), color: testing[i].background);
                spriteBatch.Draw(font12x12, destinationRectangle: new rectangle(i%testMapWidth*12, i/testMapWidth*12, 12, 12), sourceRectangle: GlyphRect(testing[i].glyph), color: testing[i].foreground);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private rectangle GlyphRect(char Character)
        {
            int cx = (Character) % fontColums;
            int cy = (Character) / fontColums;
            return new rectangle(cx*12, cy*12, 12, 12);
        }
    }
}
