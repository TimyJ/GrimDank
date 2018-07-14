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
        private Map testLevel;
        public MObjects.MObject Player { get; private set; }
        private MapRenderer mapRenderer;
        private float InputDelay;
        private float TimeSinceLastInput;
        
        // GrimDank class is the master class and as such this comment supersedes... We're testing git give me
        // a break.
        public GrimDank()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            testLevel = new Map(testMapWidth, testMapHeight);

            Coord playerSpawnPos = testLevel.WalkabilityMap.RandomPosition(true);
            Player = new MObjects.MObject(Map.Layer.CREATURES, playerSpawnPos, false, false);
            Player.glyph = '@';
            testLevel.Add(Player);
            testLevel.GenerateMap();
            
            for(int i=0; i<NUM_MOBJECTS; ++i)
            {
                testLevel.Add(new MObjects.MObject(Map.Layer.CREATURES, Coord.ToCoord(i, testLevel.Width)));
            }
            testLevel.SetupFOV(Player.Position);

            counter = new FrameCounter();
            InputDelay = 0.1f;


            mapRenderer = new MapRenderer(font12x12, testLevel);

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
            mapRenderer.CurrentFont = font12x12;
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
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (TimeSinceLastInput >= InputDelay) {
                int dx = 0;
                int dy = 0;
                foreach (int key in Keyboard.GetState().GetPressedKeys())
                    
                {
                    switch (key)
                    {
                        case (int)Keys.NumPad6: { dx = 1; TimeSinceLastInput = 0; break; }
                        case (int)Keys.NumPad4: { dx = -1; TimeSinceLastInput = 0; break; }
                        case (int)Keys.NumPad8: { dy = -1; TimeSinceLastInput = 0; break; }
                        case (int)Keys.NumPad2: { dy = 1; TimeSinceLastInput = 0; break; }
                    }
                }
                if (dx != 0 || dy != 0)
                {
                    Player.MoveIn(Direction.GetDirection(dx, dy));
                    testLevel.fov.Calculate(Player.Position, 23);
                    foreach(var pos in testLevel.fov.NewlySeen)
                    {
                        testLevel.SetExplored(true, pos);
                    }
                    mapRenderer.Camera.Area = mapRenderer.Camera.Area.NewWithCenter(Player.Position);
                }
            } else { TimeSinceLastInput += deltaTime; }

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
            
            mapRenderer.Draw(spriteBatch);
            //spriteBatch.Draw(hudTest, new rectangle(0, 0, 1280, 720), new rectangle(0, 0, 1920, 1080), Color.White);
            
            var frames = string.Format("FPS: {0}", counter.AverageFramesPerSecond);
            spriteBatch.DrawString(fpsFont, frames, new Vector2(10, 580), Color.White);
            

            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        
    }
}