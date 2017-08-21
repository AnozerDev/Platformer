﻿using System;
using Microsoft.Xna.Framework;
using OpenGL;

namespace Platformer.anozer.dmn
{
    public abstract class Enemy : Personage
    {
        protected float detectDistance;
        
        protected Enemy(Vector2 position)
        {
            this.detectDistance = 4f;
            this.position = position;
        }


        protected bool searchForPlayer(Vector2 player)
        {
            return !(Math.Abs(player.X - position.X) > detectDistance);
        }

        protected override void moveMe()
        {
            if (searchForPlayer(Platformer.playerPosition))
            {
                Console.WriteLine("I see UUU");
            }
            
        }
    }
}