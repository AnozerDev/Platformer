using System;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer.anozer.dmn;
using Platformer.anozer.dmn.@interface;
using TiledSharp;

namespace Platformer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Platformer : Game
    {

        public enum Screen
        {
            MENU,
            GAME,
            PAUSE,
            GAMEOVER
        }

        public static bool RELEASED = true;
        public static bool DEBUG = false;
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static ContentManager CONTENT_MGR;

        public const int WINDOW_WIDTH = 800;
        public const int WINDOW_HEIGHT = 600;

        private Cam cam;

        private Perso perso;
        private Player _player;
        public static Vector2 playerPosition { get; private set; }

        
        private TmxMap map;
        private Texture2D tileset;
        private int tileWidth, tileHeigth;
        private int mapWidth;
        private int tilesetColumnsCount;

        private Level level;
        private Ath ath;
        
        private Menu menu;
        private GameOver gameOver;
        
        public Screen actualScreen = Screen.MENU;

        public static String FONT_DIRECTORY; //TODO class ressources pour les gerer
        
        public Platformer()
        {
            graphics = new GraphicsDeviceManager(this);
            
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            
            Content.RootDirectory = "Content";
            FONT_DIRECTORY = "fonts/";

            CONTENT_MGR = Content;
        }

        public void backToMenu()
        {
            actualScreen = Screen.MENU;
        }
        public void startGame()
        {
            
        //    perso = new Perso();
        //    perso.load(Content, new Vector2(GraphicsDevice.Viewport.Width/2, GraphicsDevice.Viewport.Height/2));

            level = new Level(Content,"Content/map/map_1.tmx","tilesets/","tiles");
            initPlayer();
            
            cam = new Cam(level.getMapDimensions);
            
            ath = new Ath(DEBUG); 
            ath.load(Content, "fonts/");
            
            actualScreen = Screen.GAME;
            
        }

        private void initPlayer()
        {
            _player = new Player();
            _player.initPosition(level.playerStartPosition);
            
            _player.addSprite(PersoState.IDLE,
                new Sprite(Content.Load<Texture2D>("character/white_idle"), 4, 1, 0.16f, 0.05f));
            _player.addSprite(PersoState.WALK, 
                new Sprite(Content.Load<Texture2D>("character/white_walk"), 6, 1, 0.16f, 0.05f));
            _player.addSprite( PersoState.SLIP, 
                new Sprite(Content.Load<Texture2D>("character/white_slide"), 3, 1, 0.16f, 0.05f));
        }

        public void endGame()
        {
            gameOver = new GameOver(this, FONT_DIRECTORY);
            actualScreen = Screen.GAMEOVER;
        }

        public void pauseGame()
        {
            
            actualScreen = Screen.PAUSE;
        }

        public void resumeGame()
        {
            
            actualScreen = Screen.GAME;
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

            menu = new Menu(this, FONT_DIRECTORY);
            
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
            
            float deltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) && Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                Exit();

            // TODO: Add your update logic here
            switch (actualScreen)
            {
                    case Screen.GAMEOVER:
                        gameOver.update(Keyboard.GetState());
                        break;
                    case Screen.PAUSE:
                        //TODO
                        break;
                    case Screen.MENU:
                        menu.update(Keyboard.GetState());
                        break;
                    case Screen.GAME:
                        
                        _player.update(deltaTime, 0.17f, 0.37f);
                        playerPosition = _player.positionPropertie;
                        
                        if (_player.isDead && _player.positionPropertie.Y >= _player.deathPositoinPropertie.Y)
                        {
                            endGame();
                            break;
                        }
                        
                       level.update(deltaTime, _player.positionPropertie); 
                        
                        level.checkCollisions(_player.positionPropertie, _player.bottomRightPosition, _player.directionPropertie);
                        if (level.hasCollidedFloor || level.hasCollidedWall)
                        {
                            _player.collidedOn(level.collideType, level.hasCollidedWall);
                        }
                   
                        cam.update(_player.positionPropertie);
                        ath.update(_player, level, cam.getPosition);
                        
                        break;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);
            
            switch (actualScreen)
            {
                
                    case Screen.MENU:
                        spriteBatch.Begin();
                        menu.drawMenu(spriteBatch);
                        break;
                        
                    case Screen.PAUSE:
                        
                        
                    case Screen.GAME:
                        spriteBatch.Begin(SpriteSortMode.Deferred,null,null,null,null,null,cam.getMatrix);
                        
                        level.draw(spriteBatch);
            _player.draw(spriteBatch);
              //          perso.draw(spriteBatch);
                        
                        ath.draw(spriteBatch);
                        
                        break;
                        
                    case Screen.GAMEOVER:
                        spriteBatch.Begin();
                        level.draw(spriteBatch);
                        gameOver.draw(spriteBatch);
                        
                        break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
