﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rectangle = Microsoft.Xna.Framework.Rectangle;
using GoRogue.MapViews;
using GoRogue;
using System.Diagnostics;

namespace GrimDank
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 
    class GrimDank : Game
    {
        public static GrimDank Instance { get; private set; }
        // Read-only for now just to save writing property code.  But obviously changeable later.
        // Mostly these are here to get the magic constants under control for when we want to start changing
        // things.
        public readonly int WINDOW_WIDTH = 1280;
        public readonly int WINDOW_HEIGHT = 720;
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private TextureFont font12x12;
        private Texture2D hudTest;
        public Texture2D reticle;
        public SpriteFont fpsFont;
        private FrameCounter counter;
        private readonly int TEST_MAP_WIDTH = 250;
        private readonly int TEST_MAP_HEIGHT = 250;

        public Map TestLevel { get; private set; }
        public MObjects.Player Player { get; private set; }
        public MapRenderer MapRenderer { get; private set; }
        public TurnManager TurnManager { get; private set; }

        private GlobalKeyHandler _globalKeyHandler;
       
        public GrimDank()
        {
            Debug.Assert(Instance == null); // If not we've created more than one game instance and that's not good...

            Instance = this;

            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = WINDOW_WIDTH,
                PreferredBackBufferHeight = WINDOW_HEIGHT,
                SynchronizeWithVerticalRetrace = false
            };
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";

            _globalKeyHandler = new GlobalKeyHandler();

            TestLevel = new Map(TEST_MAP_WIDTH, TEST_MAP_HEIGHT);
            TestLevel.GenerateMap();

            Coord playerSpawnPos = TestLevel.WalkabilityMap.RandomPosition(true);
            Player = new MObjects.Player(playerSpawnPos);
            TestLevel.Add(Player);
            TestLevel.SetupFOV(Player.Position, Player.FOVRange);
            TestLevel.SpawnPunchingBags(100);

            TurnManager = new TurnManager(TestLevel);

            counter = new FrameCounter();

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
            var fontTexture = Content.Load<Texture2D>("font12x12");
            font12x12 = new TextureFont(fontTexture, 12, 16);
            MapRenderer = new MapRenderer(font12x12, TestLevel);
            MapRenderer.Camera.Area = MapRenderer.Camera.Area.CenterOn(Player.Position);
            MessageLog.Write("Font Loaded");

            fpsFont = Content.Load<SpriteFont>("_spritefont");

            hudTest = Content.Load<Texture2D>("BetterEnergyBar");

            reticle = Content.Load<Texture2D>("reticles");

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

            TurnManager.Update(gameTime);

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
            float playerHPPercentage = (float)Player.CurrentEnergy / (float)Player.MaxEnergy;
            //MessageLog.Write(playerHPPercentage.ToString());
            float drawPosition = 230 - (230 * playerHPPercentage);
            //MessageLog.Write(drawPosition.ToString());
            // TODO: Not sure what these magic constants really are so imma leave them.
            //TODO: fix the even more added magic constants. 
            spriteBatch.Draw(hudTest, new rectangle(8, 8, 80, 325), new rectangle(0, 0, 75, 300), Color.White);
            spriteBatch.Draw(hudTest, new rectangle(28, (int)(drawPosition + 78), 30, 230), new rectangle(75, (int)(drawPosition), 24, 230), Color.White);
            var frames = string.Format("FPS: {0}", counter.AverageFramesPerSecond);
            spriteBatch.DrawString(fpsFont, frames, new Vector2(10, 580), Color.White);
            float print = playerHPPercentage * 100;
            spriteBatch.DrawString(fpsFont, print.ToString() + "%" , new Vector2(34, 34), Color.GreenYellow);
            


            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}