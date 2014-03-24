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
        
        public Enemy(GraphicsDevice graphicsDevice, Vector3 playerLocation, float minDistance, Texture2D texture, SoundEffect soundEffect)
            : base(graphicsDevice, playerLocation, minDistance, texture, soundEffect)
        {
            collisionRadius = 1.50f;
            base.zrotation = MathHelper.WrapAngle((float)Math.PI);
        }

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

        public override void Draw(Camera camera, BasicEffect effect)
        {
            base.Draw(camera, effect);
        }
    }
}
