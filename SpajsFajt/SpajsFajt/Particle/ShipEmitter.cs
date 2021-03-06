﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class ShipEmitter:ParticleEmitter
    {
        private int particlesSinceWhite = 0;
        private int particlesSinceYellow = 0;
        public bool Boosting { get; set; }
        public bool Rainbow { get; set; }
        private int red, blue, green;
        private float rainbowElapsed;

        public ShipEmitter()
            :base(Vector2.Zero,0f)
        {
            upperBoundColor = new Color(255, 225, 1);
            lowerBoundColor = new Color(254, 20, 0);
            particleAngleDivisor = 250f;
            ParticleLifeTime = 200;
        }

        public override void GenerateParticle(int amount = 1)
        {
            if (Boosting)
                amount += 5;
            if (Rainbow)
                amount *= 7;

            for (int i = 0; i < amount; i++)
            {
                
                float r = (random.Next(-particleAngleBound, particleAngleBound)) / particleAngleDivisor;
                Color c = new Color(random.Next(lowerBoundColor.R, upperBoundColor.R), 
                    random.Next(lowerBoundColor.G, upperBoundColor.G), 
                    random.Next(lowerBoundColor.B, upperBoundColor.B));
                if (Boosting)
                    c = Color.Blue;
                if (Rainbow)
                    c = new Color(red, green, blue, 255);

                var velocity = new Vector2((float)Math.Cos(Rotation + r) * ParticleSpeed, (float)Math.Sin(Rotation + r) * ParticleSpeed);
                velocity *= ((float)random.NextDouble() + 0.5f);
                float rotVel = (random.Next(2) > 0) ? (float)random.NextDouble() / 100 : (float)-random.NextDouble() / 100;
                particles.Add(new EngineParticle(Rotation,rotVel,Position,velocity
                    ,c,ParticleLifeTime + random.Next(-100,100)));

                if (!Rainbow)
                {
                    if ((particlesSinceWhite += amount) >= 45)
                    {
                        particlesSinceWhite = 0;
                        particles.Add(new EngineParticle(Rotation, rotVel, Position, velocity, Color.White, ParticleLifeTime + random.Next(-100, 100)));
                    }
                    if ((particlesSinceYellow += amount) >= 20)
                    {
                        particlesSinceYellow = 0;
                        particles.Add(new EngineParticle(Rotation, rotVel, Position, velocity, Color.Black, ParticleLifeTime + random.Next(-100, 100)));
                    } 
                }
                
            }
        }
        
        byte a = 1;
        private int curColor = 1;
        public override void Update(GameTime gameTime)
        {
            //var ks = Keyboard.GetState();
            //if (ks.IsKeyDown(Keys.NumPad4))
            //    if (upperBoundColor.R + a < 255)
            //        upperBoundColor.R += a;

            //if (ks.IsKeyDown(Keys.NumPad1))
            //    if (upperBoundColor.R - a > 0)
            //        upperBoundColor.R -= a;

            //if (ks.IsKeyDown(Keys.NumPad5))
            //    if (upperBoundColor.G + a < 255)
            //        upperBoundColor.G += a;
            //if (ks.IsKeyDown(Keys.NumPad2))
            //    if (upperBoundColor.G - a > 0)
            //        upperBoundColor.G -= a;

            //if (ks.IsKeyDown(Keys.NumPad6))
            //    if (upperBoundColor.B + a < 255)
            //        upperBoundColor.B += a;
            //if (ks.IsKeyDown(Keys.NumPad3))
            //    if (upperBoundColor.B - a > 0)
            //        upperBoundColor.B -= a;

            //if (ks.IsKeyDown(Keys.U))
            //    if (lowerBoundColor.R + a < 255)
            //        lowerBoundColor.R += a;
            //if (ks.IsKeyDown(Keys.J))
            //    if (lowerBoundColor.R - a > 0)
            //        lowerBoundColor.R -= a;

            //if (ks.IsKeyDown(Keys.I))
            //    if (lowerBoundColor.G + a < 255)
            //        lowerBoundColor.G += a;
            //if (ks.IsKeyDown(Keys.K))
            //    if (lowerBoundColor.G - a > 0)
            //        lowerBoundColor.G -= a;

            //if (ks.IsKeyDown(Keys.O))
            //    if (lowerBoundColor.B + a < 255)
            //        lowerBoundColor.B += a;
            //if (ks.IsKeyDown(Keys.L))
            //    if (lowerBoundColor.B - a > 0)
            //        lowerBoundColor.B -= a;

            if (Rainbow)
            {
                rainbowElapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (rainbowElapsed > 10)
                {
                    rainbowElapsed = 0;
                    if (green <= 253 && curColor == 1)
                    {
                        green += 1;
                        if (green > 253)
                            curColor = 2;
                    }
                    else if (blue <= 253 && curColor == 2)
                    {

                        blue += 1;
                        green = 0;
                        if (blue > 253)
                            curColor = 3;
                    }
                    else if (red <= 253 && curColor == 3)
                    {
                        red += 1;
                        green = 0;
                        blue = 0;
                        if (red > 253)
                            curColor = 4;
                    }
                    else if (curColor == 4)
                    {
                        if (blue < 155)
                            blue += 2;
                        else if (green < 100)
                            green += 1;
                        else
                            curColor = 5;
                       
                    }
                    else
                    {
                        green = red = blue = 0;
                        curColor = 1;
                    }
                }
            }

            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.DrawString(TextureManager.GameFont, lowerBoundColor.ToString(), new Vector2(Position.X, Position.Y - 100), Color.White,0f,Vector2.Zero,1f,SpriteEffects.None,1f);
            //spriteBatch.DrawString(TextureManager.GameFont, upperBoundColor.ToString(), new Vector2(Position.X, Position.Y - 120), Color.White,0f,Vector2.Zero,1f,SpriteEffects.None,1f);

            base.Draw(spriteBatch);
        }
    }
}
