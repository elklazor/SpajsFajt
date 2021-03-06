﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class Projectile:GameObject
    {
        public int SenderID { get; set; }
        public bool FiredByAI { get; set; }
        private float lifeTime;
        public float SpeedOffset { get; set; }
        public Projectile(int id,float rot,Vector2 pos):base("projectile",id)
        {
            Position = pos;
            Rotation = rot;
            velocity = 10f;
            origin = new Vector2(1, 3);
            collisionRectangle.Width = 6;
            collisionRectangle.Height = 10;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.SpriteSheet,position,TextureRectangle,Color.Yellow,Rotation + (float)Math.PI/2,origin,2f,SpriteEffects.None,0.5f);
        }
        public override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
        }
        public void Move()
        {
            position += new Vector2((float)Math.Cos(rotation) * velocity*SpeedOffset, (float)Math.Sin(rotation) * velocity*SpeedOffset);
        }
        public void UpdateTime(GameTime gameTime)
        {
            lifeTime += gameTime.ElapsedGameTime.Milliseconds;
            if (lifeTime > 2000)
                Dead = true;
        }

    }
}
