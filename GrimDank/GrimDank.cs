using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using rectangle = Microsoft.Xna.Framework.Rectangle;
using GoRogue.MapViews;
using GoRogue;

namespace GrimDank
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 

    class GrimDank : Game
    {

        static readonly int NUM_MOBJECTS = 200;
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Texture2D font12x12;
        private Texture2D hudTest;
        private SpriteFont fpsFont;
        private FrameCounter counter;
        private static int testMapWidth = 250;
        private static int testMapHeight = 250;
        public static Map TestLevel { get; private set; }
        public static MObjects.MObject Player { get; private set; }
        public static MapRenderer MapRenderer { get; private set; }
        
        // GrimDank class is the master class and as such this comment supersedes... We're testing git give me
        // a break!
        public GrimDank()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            TestLevel = new Map(testMapWidth, testMapHeight);

            Coord playerSpawnPos = TestLevel.WalkabilityMap.RandomPosition(true);
            Player = new MObjects.MObject(Map.Layer.CREATURES, playerSpawnPos, false, false);
            Player.glyph = '@';
            TestLevel.Add(Player);
            TestLevel.GenerateMap();
            
            for(int i=0; i<NUM_MOBJECTS; ++i)
            {
                TestLevel.Add(new MObjects.MObject(Map.Layer.CREATURES, Coord.ToCoord(i, TestLevel.Width)));
            }
            TestLevel.SetupFOV(Player.Position);

            counter = new FrameCounter();

            MapRenderer = new MapRenderer(font12x12, TestLevel);

            IsFixedTimeStep = false;

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
            MapRenderer.CurrentFont = font12x12;
            MessageLog.Write("Font Loaded");

            fpsFont = Content.Load<SpriteFont>("_spritefont");

            hudTest = Content.Load<Texture2D>("TESTHUD");

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
            InputStack.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            counter.Update(deltaTime);
            
            //test drawing
            
            spriteBatch.Begin();
            
            MapRenderer.Draw(spriteBatch);
            //spriteBatch.Draw(hudTest, new rectangle(0, 0, 1280, 720), new rectangle(0, 0, 1920, 1080), Color.White);
            
            var frames = string.Format("FPS: {0}", counter.AverageFramesPerSecond);
            spriteBatch.DrawString(fpsFont, frames, new Vector2(10, 580), Color.White);
            

            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        
    }
}