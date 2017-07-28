using System;
using System.Diagnostics.Tracing;
using System.Security.Principal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platformer.anozer.dmn
{
    enum PersoState
    {
        IDLE , WALK, JUMP
    }
    
    public class Perso
    {
        private Texture2D idleTexture;
        private Texture2D walkTexture;
        
        private Rectangle [] idleFrames;
        private Rectangle[] walkFrames;
        
        private int currentFrameID;

        private const float WALK_SPEED = 0.015f;
        private const float IDLE_SPEED = 0.16f;
        
        private double animationTimer;
        private float animationSpeed = IDLE_SPEED;
        
        
        private Vector2 position = Vector2.Zero;
        private float moveSpeed = 160f; 
        private float gravity = 5; //TODO jumpVelocity
        private Vector2 velocity;

        private PersoState state;
        private bool isJumping = false;

        private Texture2D textureToDraw;
        private Rectangle[] framesToAnime;

        private SpriteEffects spriteEffect;
        

        
        public Perso()
        {
            state = PersoState.IDLE;
        }

        private void changeState(PersoState paramState)
        {
            if (!paramState.Equals(state))
            {
                switch (paramState)
                {
                    case PersoState.IDLE:
                        state = PersoState.IDLE;
                        textureToDraw = idleTexture;
                        framesToAnime = idleFrames;

                        animationSpeed = IDLE_SPEED;
                        
                        break;
                        
                    case PersoState.WALK:
                        state = PersoState.WALK;
                        textureToDraw = walkTexture;
                        framesToAnime = walkFrames;

                        animationSpeed = WALK_SPEED;
                        
                        break;
                        
                    case PersoState.JUMP:
                        state = PersoState.JUMP;
                        isJumping = true;
                        
                        //gravity = 600 * 0.16f; // TODO voir avec le deltatime
                        break;
                        
                    default:
                        changeState(PersoState.IDLE);
                        break;
                                
                }

                currentFrameID = 0;

            }
        }
        
        /// <summary>
        /// Incremente la frame à animer (Rectangle[i]) en fonction du deltaTime.
        /// /!\ Necessite variables animation speed et animationTimer dans la class hote.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="framesToAnime"></param>
        private void animeIt(double deltaTime, Rectangle [] framesToAnime)
        {
            animationTimer += deltaTime;
            if (animationTimer >= animationSpeed)
            {
                currentFrameID++;
                if (currentFrameID >= framesToAnime.Length) currentFrameID = 0;
                
                animationTimer = 0;
            }
        }

        /// <summary>
        /// Renvois un tableau de rectangles comprennan les positions de chaque frames d' un spritesheet
        /// </summary>
        /// <param name="nbColumns">Le nombre de frames par lignes</param>
        /// <param name="nbLines">Le nombre de frames par colonnes</param>
        /// <param name="textureToSplit">Le spritesheet à couper</param>
        /// <returns>Rectangle [] de la taille du nombre d'images</returns>
        private Rectangle[] getFramesFromTexture(int nbColumns, int nbLines, Texture2D textureToSplit)
        {
            int framesWidth, framesHeight;
            Rectangle[] frames;
            int frameID;
            
            framesWidth = textureToSplit.Width / nbColumns;
            framesHeight = textureToSplit.Height / nbLines;
            
            frames = new Rectangle[nbColumns*nbLines];
            frameID = 0;

            for (int l = 0; l < nbLines; l++)
            {
                for (int c = 0; c < nbColumns; c++)
                {
                    frames[frameID++] = new Rectangle(
                        new Point( c*framesWidth, l*framesHeight),
                        new Point( framesWidth, framesHeight));
                }
            }
            
            return frames;
        }
        
        public void load(ContentManager contentMgr, Vector2 persoPosition)
        {
            this.position = persoPosition;
            // IDLE
            idleTexture = contentMgr.Load<Texture2D>("skeleton/spritesheet_idle_r");
            idleFrames = getFramesFromTexture(4, 3, idleTexture);
            
            // WALK
            walkTexture = contentMgr.Load<Texture2D>("skeleton/spritesheet_walk_r");
            walkFrames = getFramesFromTexture(4, 4, walkTexture);

            textureToDraw = idleTexture;
            framesToAnime = idleFrames;
            currentFrameID = 0;
            
        }
        
        public void update(MouseState mouse, KeyboardState keyboard, GameTime gameTime)
        {
         
            double deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            position -= velocity * gravity;
            
            if (keyboard.IsKeyDown(Keys.Left))
            {
                changeState(PersoState.WALK);
                spriteEffect = SpriteEffects.FlipHorizontally;

                position.X -= moveSpeed * Convert.ToSingle(deltaTime);
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                changeState(PersoState.WALK);
                spriteEffect = SpriteEffects.None;

                position.X += moveSpeed * Convert.ToSingle(deltaTime);

            }
            else
            {
                changeState(PersoState.IDLE);
            }
            
            if (!isJumping && keyboard.IsKeyDown(Keys.Space))
            {
                //TODO sauter
            //    changeState(PersoState.JUMP);
                isJumping = true;
                velocity.Y = -0.05f;

                position.Y -= 10f;
                    //velocity.Y * Convert.ToSingle(deltaTime);
            }


            if (isJumping)
            {
                velocity.Y += 0.5f * Convert.ToSingle(deltaTime);
            }

            if (isJumping && position.Y + framesToAnime[currentFrameID].Height >= 350) // TODO maxHeightToJump
            {
                isJumping = false; //TODO mettre à false qu' une fois r' atteri
                velocity.Y = -0.8f;
            }
            
            animeIt(deltaTime, framesToAnime);
        }

        public void draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(
                textureToDraw, // la Texture
                position, // la position sur l' ecran
                framesToAnime[currentFrameID], // rectangle source
                Color.White, // Couleur de teinte, white ne color pas
                0f, // rotation en radians
                new Vector2(0, 0), //origin du sprite 0,0 par default
                new Vector2(0.1f), // echelle du sprite TODO mise à l' echelle en fonction de la taille de l'ecran
                spriteEffect, // Effet sur le sprite
                0 // layerDeph (z-index)
                );
        }
    }
}