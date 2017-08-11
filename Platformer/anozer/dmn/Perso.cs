using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platformer.anozer.dmn
{
    enum PersoState
    {
        IDLE , WALK, JUMP, FALL, SLIP
    }

    public enum Direction
    {
        RIGHT, LEFT
    }

    public class Perso
    {
        private Texture2D idleTexture;
        private Texture2D walkTexture;
        
        private Rectangle [] idleFrames;
        private Rectangle[] walkFrames;
        
        private int currentFrameID;
        private Vector2 scale = new Vector2(0.15f);

        private const float WALK_SPEED = 0.015f;
        private const float IDLE_SPEED = 0.16f;
        
        private double animationTimer;
        private float animationSpeed = IDLE_SPEED;

        private double deltaTime;
        
        
        private Vector2 position = Vector2.Zero;
        public Vector2 positionPropertie
        {
            get => position;
            set => position = value;
        }

        private float feetPosition;
        private float rightPosition;
        
        public Vector2 bottomRightPosition => new Vector2(rightPosition,feetPosition);

        private Vector2 oldPos;
        // Pour ne faire la sauvgarde de l' oldPos qu' une fois sur deux
        private bool halfUpdate = true; // vraiment pas mieux ?
        
        private float moveSpeed = 160f; 
        private float gravity = 0.1f;
        //private float friction = 0.1f;
        private Vector2 velocity;
        public Vector2 getVelocity => velocity;

        private PersoState state;
        public String getState => state.ToString();
        
        public Direction directionPropertie { get; private set; }
        
        private bool isJumping = false;
        public bool isJumpingPropertie => isJumping;

        private Texture2D textureToDraw;
        private Rectangle[] framesToAnime;

        private SpriteEffects spriteEffect;
        
        private int floorY = -1; // hauteur du sol
        //TODO ajouter les points de collisions aux sprites !! (un calque avec pixel transparent sur l' image ?? ))

        
        public Perso()
        {
            state = PersoState.IDLE;
        }

        private void changeState(PersoState paramState)
        {
            if ( !isJumping && !paramState.Equals(state))
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
                        state = state != PersoState.FALL ? PersoState.WALK : state;
                        textureToDraw = walkTexture;
                        framesToAnime = walkFrames;

                        animationSpeed = WALK_SPEED;
                        
                        break;
                        
                    case PersoState.JUMP:
                        state = PersoState.JUMP;
                        isJumping = true;
                        break;
                        
                    case PersoState.FALL:
                        state = PersoState.FALL;
                        isJumping = false;
                        
                        break;
                    
                    case PersoState.SLIP:
                        state = PersoState.SLIP;
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
    //        floorY = (int) persoPosition.Y; // TODO en admettant qu' il soit bien positionné au depart :fp:
            
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
        private void jump()
        {
            velocity.Y = -3.8f;
        }

        public bool isCollide()
        {
            return false;
        }

        public void slip(Direction sens, double deltaTime)
        {
            //TODO checker la collision bas gauche si vas à droite et inverse !!
            var slipSpeed = Convert.ToSingle(moveSpeed * 1.5 * deltaTime);
            position.X += (sens == Direction.RIGHT)? slipSpeed : - slipSpeed;
            //position.X += 1;
            position.Y = oldPos.Y + slipSpeed;
            changeState(PersoState.SLIP);
            directionPropertie = sens;
        }

        //TODO "collided(CollideType, CollidePosition)" plutot 
        public void returnOldPos(Level.CollideType collideType, bool collidedWall) // TODO plutot qu' un type de collision, c' serait pas mieux la position du collide ?!?
        {
            if (collidedWall)
            {
                position.X = oldPos.X;
                //TODO faire chuter s' il esst en saut ?
            }
            
            switch (collideType)
            {
                case Level.CollideType.NONE:
                    break;
                    
                case Level.CollideType.BOTTOM:

                    if (isJumping) break;
                    
                    if (!isJumping && state == PersoState.FALL)
                    {
                        velocity.Y = 0;
                        position.Y = oldPos.Y;
                        changeState(PersoState.IDLE);
                    }
                    else
                    {
                        position.Y = oldPos.Y;
                        break;
                    }
                    break;
                    
                case Level.CollideType.DESCENT_LEFT:
                    slip(Direction.LEFT, deltaTime);
                    break;
                case Level.CollideType.DESCENT_RIGHT:
                    slip(Direction.RIGHT, deltaTime);
                    break;
                default:
                    position = oldPos;
                    break;
            }
        }
        
        public void update(MouseState mouse, KeyboardState keyboard, GameTime gameTime)
        {
            oldPos = position;
            
            feetPosition = position.Y + framesToAnime[currentFrameID].Height * scale.Y;
            rightPosition = position.X + framesToAnime[currentFrameID].Width * scale.X;
         
            deltaTime = gameTime.ElapsedGameTime.TotalSeconds;

            velocity.Y += gravity;
            //velocity.X += friction;
            position += velocity;
            
            // Chute si trop grande marge entre les deux positions
            //Pas utile de verifier si dejà idle ?
            if (state != PersoState.IDLE)
            {
                
                if ( state != PersoState.SLIP && velocity.Y > 0 && Math.Abs(position.Y - oldPos.Y) > 1.22)
                {
                    isJumping = false;
                    changeState(PersoState.FALL);
                }
            }
            
            /*
            *--------    INPUTS TODO depuis quand c' est le perso qui ecoute les inputs ??
            */
            // TODO utiliser la velocity x plutot que de modifier directement la position !!
            if (keyboard.IsKeyDown(Keys.Left))
            {
                changeState(PersoState.WALK);
                spriteEffect = SpriteEffects.FlipHorizontally;

                position.X -= moveSpeed * Convert.ToSingle(deltaTime);
                directionPropertie = Direction.LEFT;
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                changeState(PersoState.WALK);
                spriteEffect = SpriteEffects.None;

                position.X += moveSpeed * Convert.ToSingle(deltaTime);
                directionPropertie = Direction.RIGHT;

            }
            else
            {
                if ( state != PersoState.IDLE && state != PersoState.FALL && state != PersoState.JUMP)
                {
                    changeState(PersoState.IDLE);
                }
            }

            //TODO remplacé par le collideType
            //TODO pas utile de conserver le floorY du coup ??
            /*if (floorY != -1 && floorY < position.Y + framesToAnime[currentFrameID].Height * scale.Y)
            {
                velocity.Y = 0;
                position.Y = floorY - framesToAnime[currentFrameID].Height * scale.Y;
                isJumping = false;
            }*/
 

            if (state != PersoState.FALL && !isJumping && keyboard.IsKeyDown(Keys.Space))
            {
                changeState(PersoState.JUMP);
                jump();
            }
            /*
            TODO creer un arret dans les slide si on ne bouge pas.. 'fin des fois.. choper un autre moyen pour regul la velocity
            TODO mais sans la velocity.Y ne fait qu' augmenter, et l' on se retrouve sous le sol en sautant !
            if (state == PersoState.IDLE)
            {
                velocity = Vector2.Zero;
            }*/

//Console.WriteLine("{0}, {1}..{2}  velo == {3} // {4}",state, position, oldPos, velocity, isJumping);

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
                scale, // echelle du sprite TODO mise à l' echelle en fonction de la taille de l'ecran
                spriteEffect, // Effet sur le sprite
                0 // layerDeph (z-index)
                );

            if (Platformer.DEBUG)
            {
                //Position ( RED ) => topLeft
                Color[] data = new Color[5*5];
                Texture2D rect = new Texture2D(spriteBatch.GraphicsDevice, 5, 5);
                for (int i = 0; i < data.Length; i++) data[i] = Color.Red;
                rect.SetData(data);
                spriteBatch.Draw(rect, position,Color.White);
                
                //bottomRight (GREEN) => bottomRight
                Texture2D rectBR = new Texture2D(spriteBatch.GraphicsDevice, 5, 5);
                for (int i = 0; i < data.Length; i++) data[i] = Color.Green;
                rectBR.SetData(data);
                spriteBatch.Draw(rectBR, bottomRightPosition,Color.White);
                
                //Collide bottom (YELLOW) => bottomCenter !!pied avant du skeleton
                Texture2D rectBOTTOM = new Texture2D(spriteBatch.GraphicsDevice, 5, 5);
                for (int i = 0; i < data.Length; i++) data[i] = Color.Yellow;
                rectBOTTOM.SetData(data);
                spriteBatch.Draw(rectBOTTOM,new Vector2(position.X + (bottomRightPosition.X - position.X) / 2, bottomRightPosition.Y),Color.White);
                
                //left
                spriteBatch.Draw(rectBOTTOM,new Vector2(position.X, Convert.ToSingle(position.Y + (bottomRightPosition.Y - position.Y) / 3) ), Color.White);
                spriteBatch.Draw(rectBOTTOM,new Vector2(position.X, Convert.ToSingle(position.Y + (bottomRightPosition.Y - position.Y) / 1.15) ), Color.White);
            }
        }
    }
}