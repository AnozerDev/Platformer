using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer.anozer.dmn
{
    public enum PersoState
    {
        IDLE, WALK, RUN, JUMP, FALL, SLIP, DEAD, ATTACK
    }

    public enum Direction
    {
        RIGHT, LEFT
    }
    
    public abstract class Personage
    {
        private Dictionary<PersoState, Sprite> _spritesList ;
        private Sprite _currentSprite;

        protected Vector2 position;
        public Vector2 positionPropertie => position;

        public Vector2 bottomRightPosition => //TODO euh.. tu tien pas compte de l' origine là ?
            new Vector2(position.X + _currentSprite.sizePropertie.X * _currentSprite.scalePropertie,
                position.Y + _currentSprite.sizePropertie.Y * _currentSprite.scalePropertie);
        
        protected Vector2 oldPosition;
        
        protected Vector2 velocity;
        public Vector2 velocityPropertie => velocity;
        
        protected Vector2 maxVelocity;
        protected float moveSpeed;

        protected float deltaTime { get; private set; }
        
        protected PersoState state;
        public PersoState statePropertie => state;
        public bool isJumping => state == PersoState.JUMP;
        
        protected Direction direction;
        public Direction directionPropertie => direction;

        protected Vector2 deathPosition;
        public Vector2 deathPositoinPropertie => deathPosition;
        public bool isDead => state == PersoState.DEAD;
        
        protected Personage() :this(new Dictionary<PersoState, Sprite>())
        {
        }

        protected Personage(Dictionary<PersoState, Sprite> spritesForStates)
        {
            _spritesList = spritesForStates;
            
            state = PersoState.IDLE;
            moveSpeed = 60f;
            maxVelocity = new Vector2(6f, 10f);
        }

        protected abstract void changeState(PersoState state);

        protected abstract void dies();
        
        protected abstract void jump();
        protected abstract void walk(Direction direction, float step);
        
        public void initPosition(Vector2 position)
        {
            this.position = position;
        }

        public void addSprite(PersoState state, Sprite sprite)
        {
            if (_spritesList.Count == 0) _currentSprite = sprite;
            
            _spritesList.Add(state, sprite);
        }

        protected bool isFalling()
        {//TODO euh.. et velocity > gravity c'est plus simple !!
            if (state == PersoState.IDLE || state == PersoState.SLIP) return false;
            if (!(velocity.Y > 0) || !(Math.Abs(position.Y - oldPosition.Y) > 1.22)) return false;
            
            changeState(PersoState.FALL);
            return true;
        }

        protected virtual void slip(Direction direction, float slipSpeed)
        {
            this.direction = direction;
            
            position.X += (this.direction == Direction.LEFT) ? -slipSpeed : slipSpeed;
            position.Y = oldPosition.Y + slipSpeed;
            changeState(PersoState.SLIP);
        }

        protected abstract void moveMe();
        
        public virtual void update(float deltaTime, float gravity, float friction)
        {
            this.deltaTime = deltaTime;
            
            oldPosition = position;

            if (velocity.X != 0)
            {
                if ((direction == Direction.LEFT && velocity.X < 0) || (direction == Direction.RIGHT && velocity.X > 0))
                {
                    friction = state == PersoState.FALL ? friction / 4 : friction;
                    velocity.X += (direction == Direction.LEFT) ? friction : -friction;
                    
                    if (Math.Abs(velocity.X) > maxVelocity.X)
                    {
                        velocity.X = velocity.X > 0 ? maxVelocity.X : -maxVelocity.X;
                    }
                }
                else
                {
                    velocity.X = 0;
                }
            }
            velocity.Y += gravity;
            
            if (!isDead)
            {
                if (velocity.Y <= gravity && state != PersoState.SLIP)
                {
                    moveMe();
                }
            }
            
            position += velocity;
            
            if(!isDead) isFalling();
            
            _currentSprite = (_spritesList.ContainsKey(state))?
                _spritesList[state] : _spritesList[PersoState.IDLE];
            _currentSprite.update(this.deltaTime,
                direction == Direction.RIGHT ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

        }
        
        public virtual void draw(SpriteBatch spriteBatch)
        {
            _currentSprite.draw(spriteBatch, position);
            
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
                
                spriteBatch.Draw(rect, new Vector2(position.X, bottomRightPosition.Y),Color.CadetBlue);
            }
        }

        public void collidedOn(Level.CollideType collideType, bool onWall)
        {
            if (state == PersoState.DEAD) return;
            if (onWall) position.X = oldPosition.X;

            switch (collideType)
            {
                case Level.CollideType.NONE:
                    //bah pourquoi t' y passe ??
                    break;
                    
                case Level.CollideType.DEADLY:
                    dies();
                    break;
                    
                case Level.CollideType.BOTTOM:
                    if (state == PersoState.JUMP) break;
                    
                    position.Y = oldPosition.Y;
                    
                    if (state == PersoState.FALL || state == PersoState.SLIP)
                    {
                        changeState(PersoState.IDLE);
                    }
                    velocity.Y = 0;
                    break;
                    
                case Level.CollideType.DESCENT_LEFT:
                    slip(Direction.LEFT, moveSpeed * deltaTime * 5f);
                    break;
                
                case Level.CollideType.DESCENT_RIGHT:
                    slip(Direction.RIGHT, moveSpeed * deltaTime * 5f);
                    break;
                    
                default:
                    position = oldPosition;
                    break;
            }
        }
        
    }
}