/***********************************************************************
 * Class: Enemy
 * Description: Derives from Cube class, acts as an enemy in the game
 * Author(s): Jonah Ahvonen, Matti Jokitulppo
 * Date: April 1, 2014
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace ErppuTribute
{
    class Enemy : Cube
    {
        #region Constructor
        public Enemy(GraphicsDevice graphicsDevice, Vector3 playerLocation, float minDistance, float enemyCollisionRadius,Texture2D texture, SoundEffect soundEffect)
            : base(graphicsDevice, playerLocation, minDistance, enemyCollisionRadius, texture, soundEffect)
        {
            base.zrotation = MathHelper.WrapAngle((float)Math.PI);
        }
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            base.rotation = MathHelper.WrapAngle(rotation + 0.01f);

            float minY = 0.4f;
            float maxY = 0.6f;
            float yAmount = 0.001f;
            int dir = 0;

            if (dir == 0)
            {
                if (this.location.Y - yAmount < minY)
                {
                    dir = 1;
                }
                else
                {
                    this.location = new Vector3(this.location.X, this.location.Y - yAmount, this.location.Z);
                }
            }
            else
            {
                if (this.location.Y + yAmount > maxY)
                {
                    dir = 0;
                }
                else
                {
                    this.location = new Vector3(this.location.X, this.location.Y + yAmount, this.location.Z);
                }
            }

            
            //base.Update(gameTime);
        }

        public void PositionEnemy(Vector3 PlayerLocation, float minDistance, float maxDistance)
        {
            Vector3 newLocation;
            do
            {
                newLocation = new Vector3(rand.Next(0, Maze.mazeWidth) + 0.5f, 0.5f, rand.Next(0, Maze.mazeHeight) + 0.5f);
            }
            while (Vector3.Distance(PlayerLocation, newLocation) < minDistance && Vector3.Distance(PlayerLocation, newLocation) > maxDistance);

            location = newLocation;
        }

        public override void Draw(Camera camera, BasicEffect effect)
        {
            base.Draw(camera, effect);
        }
        #endregion
    }
}
