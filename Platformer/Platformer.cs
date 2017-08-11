using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer.anozer.dmn;
using TiledSharp;

namespace Platformer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Platformer : Game
    {
        public static bool DEBUG = true;
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public const int WINDOW_WIDTH = 800;
        public const int WINDOW_HEIGHT = 600;

        private Perso perso;

        private TmxMap map;
        private Texture2D tileset;
        private int tileWidth, tileHeigth;
        private int mapWidth;
        private int tilesetColumnsCount;

        private Level level;
        private Ath ath;
        
        public Platformer()
        {
            graphics = new GraphicsDeviceManager(this);
            
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            
            Content.RootDirectory = "Content";
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

            // TODO: use this.Content to load your game content here
            
            perso = new Perso();
            perso.load(Content, new Vector2(GraphicsDevice.Viewport.Width/2, GraphicsDevice.Viewport.Height/2));
            //OLD perso.load(Content, new Vector2(50, 50));

            level = new Level(Content,"Content/map/map_1.tmx","tilesets/","tiles");

            ath = new Ath(DEBUG);
            ath.load(Content, "fonts/");
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
            
            perso.update(Mouse.GetState(), Keyboard.GetState(), gameTime);
            level.update(perso.positionPropertie, perso.bottomRightPosition, perso.directionPropertie);
            if (level.hasCollidedFloor || level.hasCollidedWall)
            {
                perso.returnOldPos(level.collideType, level.hasCollidedWall);
            }
            
            
            ath.update(perso, level);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            // TODO: Add your drawing code here
            
            //Draw le skeleton
            spriteBatch.Begin();
            
            level.draw(spriteBatch);
            perso.draw(spriteBatch);
            
            ath.draw(spriteBatch);
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
