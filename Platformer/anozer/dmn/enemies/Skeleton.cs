using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer.anozer.dmn.enemies
{
    
    public class Skeleton : Enemy
    {
        private float _scale = 0.15f;

        protected override void changeState(PersoState state)
        {
            if (this.state == state) return;
            switch (state)
            {
                case PersoState.IDLE:
                    this.state = PersoState.IDLE;
                    break;
                
                case PersoState.WALK:
                    this.state = PersoState.WALK;
                    break;
                    
                default:
                    this.state = PersoState.IDLE;
                    break;
            }
        }

        protected override void dies()
        {
            changeState(PersoState.DEAD);
        }

        protected override void jump()
        {
            throw new System.NotImplementedException();
        }

        protected override void walk(Direction direction, float step)
        {
            this.direction = direction;
            velocity.X += (this.direction == Direction.LEFT) ? -step : step;
        }

        public Skeleton(Vector2 position) : base(position)
        {
            this.addSprite(PersoState.IDLE,
                new Sprite(Platformer.CONTENT_MGR.Load<Texture2D>("skeleton/spritesheet_idle_r"), 4, 3, 0.16f, _scale));
            this.addSprite(PersoState.WALK,
                new Sprite(Platformer.CONTENT_MGR.Load<Texture2D>("skeleton/spritesheet_walk_r"), 4, 4, 0.16f, _scale));
            
        }
    }
}