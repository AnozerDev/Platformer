using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platformer.anozer.dmn
{
    
    public class Player :Personage
    {
        public Player()
        {
        }

        protected override void changeState(PersoState state)
        {
            if (this.state == state) return;
            if (this.state == PersoState.JUMP && state != PersoState.FALL) return;
            
            switch (state)
            {
                case PersoState.IDLE:
                    this.state = PersoState.IDLE;
                        
                    break;
                case PersoState.WALK:
                    this.state = PersoState.WALK;
                    break;
                        
                case PersoState.JUMP:
                    this.state = PersoState.JUMP;
                    break;
                    
                case PersoState.FALL:
                    this.state = PersoState.FALL;
                    break;
                        
                case PersoState.SLIP:
                    this.state = PersoState.SLIP;
                    break;
                    
                case PersoState.DEAD:
                    this.state = PersoState.DEAD;
                    break;
                    
                default:
                    this.state = PersoState.IDLE;
                    break;
            }
        }

        protected override void dies()
        {
            deathPosition = position;
            
            velocity.Y = -9f;
            changeState(PersoState.DEAD);
        }

        protected override void jump()
        {
            velocity.Y = -5.8f;
            
        }

        protected override void walk(Direction direction, float step)
        {
            this.direction = direction;
            velocity.X += (this.direction == Direction.LEFT) ? -step : step;
        }


        protected override void moveMe()
        {
            KeyboardState keyboard = Keyboard.GetState();
            float step = moveSpeed * deltaTime;
            
            if (state != PersoState.JUMP && keyboard.IsKeyDown(Keys.Space))
            {
                changeState(PersoState.JUMP);
                jump();
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                changeState(PersoState.WALK);
                walk(Direction.LEFT, step);
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                changeState(PersoState.WALK);
                walk(Direction.RIGHT, step);
            }
            else 
            {
                if (state != PersoState.JUMP || state != PersoState.FALL || state != PersoState.SLIP) 
                {
                    changeState(PersoState.IDLE);
                }
            }
        }

        public void update(float deltaTime, float gravity, float friction)
        {
            base.update(deltaTime, gravity, friction);
Console.WriteLine("{0}, {1}" , position, state);                        
        }

        public void draw(SpriteBatch spriteBatch)
        {
            base.draw(spriteBatch);
        }
    }
}