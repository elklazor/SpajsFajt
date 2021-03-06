﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpajsFajt
{
    class Modifiers
    {
        private int PlayerID;

        public Shield Shield
        {
            get; set;
        }

        public WeaponLevel WeaponLevel = WeaponLevel.One;
        public ProjectileLevel ProjectileLevel = ProjectileLevel.One;
        public EngineLevel EngineLevel = EngineLevel.One;
        public bool Rainbow { get; set; }

        public Modifiers(int id)
        {
            PlayerID = id;
            Shield = new Shield();
        }

        public float SpeedModification()
        {
            var mod = 1.0f;
            switch (EngineLevel)
            {
                case EngineLevel.One:
                    break;
                case EngineLevel.Two:
                    mod = 1.15f;
                    break;
                case EngineLevel.Three:
                    mod = 1.3f;
                    break;
            }
            return mod;
        }
        
        public float ProjectileSpeedModification()
        {
            var mod = 1.0f;
            switch (ProjectileLevel)
            {
                case ProjectileLevel.One:
                    break;
                case ProjectileLevel.Two:
                    mod = 1.15f;
                    break;
                case ProjectileLevel.Three:
                    mod = 1.25f;
                    break;
                case ProjectileLevel.Four:
                    mod = 1.35f;
                    break;
            }
            return mod;
        }

        public List<Projectile> GetProjectiles(Vector2 pos, float rot)
        {
            var proj = new List<Projectile>();
            switch (WeaponLevel)
            {
                case WeaponLevel.One:
                    proj.Add(new Projectile(GameServer.NextID(), rot, pos) { SenderID = PlayerID, SpeedOffset = ProjectileSpeedModification() });
                    break;
                case WeaponLevel.Two:
                    proj.Add(new Projectile(GameServer.NextID(), rot - (MathHelper.Pi / 30), pos) { SenderID = PlayerID, SpeedOffset = ProjectileSpeedModification()});
                    proj.Add(new Projectile(GameServer.NextID(), rot + (MathHelper.Pi / 30), pos) { SenderID = PlayerID, SpeedOffset = ProjectileSpeedModification() });
                    break;
                case WeaponLevel.Three:
                    proj.Add(new Projectile(GameServer.NextID(), rot - (MathHelper.Pi / 20), pos) { SenderID = PlayerID, SpeedOffset = ProjectileSpeedModification() });
                    proj.Add(new Projectile(GameServer.NextID(), rot, pos) { SenderID = PlayerID, SpeedOffset = ProjectileSpeedModification() });
                    proj.Add(new Projectile(GameServer.NextID(), rot + (MathHelper.Pi / 20), pos) { SenderID = PlayerID, SpeedOffset = ProjectileSpeedModification() });
                    break;
                case WeaponLevel.Four:
                    proj.Add(new Projectile(GameServer.NextID(), rot - (MathHelper.Pi / 20), pos) { SenderID = PlayerID, SpeedOffset = ProjectileSpeedModification() });
                    proj.Add(new Projectile(GameServer.NextID(), rot - (MathHelper.Pi / 50), pos) { SenderID = PlayerID, SpeedOffset = ProjectileSpeedModification() });
                    proj.Add(new Projectile(GameServer.NextID(), rot + (MathHelper.Pi / 50), pos) { SenderID = PlayerID, SpeedOffset = ProjectileSpeedModification() });
                    proj.Add(new Projectile(GameServer.NextID(), rot + (MathHelper.Pi / 20), pos) { SenderID = PlayerID, SpeedOffset = ProjectileSpeedModification() });
                    break;
            }

            return proj;
        }
        public bool Modify(int i)
        {
            bool ret = false;
            switch (i)
            {
                case 1:
                    if (WeaponLevel == WeaponLevel.One)
                    {
                        WeaponLevel = WeaponLevel.Two;
                        ret = true;
                    }
                    break;
                case 5:
                    if (WeaponLevel == WeaponLevel.Two)
                    {
                        WeaponLevel = WeaponLevel.Three;
                        ret = true;
                    }
                    break;
                case 9:
                    if (WeaponLevel == WeaponLevel.Three)
                    {
                        WeaponLevel = WeaponLevel.Four;
                        ret = true;
                    }
                    break;
                case 2:
                    if (Shield.Level == ShieldEnum.None)
                    {
                        Shield.Level = ShieldEnum.One;
                        ret = true;
                    }
                    break;
                case 6:
                    if (Shield.Level == ShieldEnum.One)
                    {
                        Shield.Level = ShieldEnum.Two;
                        ret = true;
                    }
                    break;
                case 10:
                    if (Shield.Level == ShieldEnum.Two)
                    {
                        Shield.Level = ShieldEnum.Three;
                        ret = true;
                    }
                    break;
                case 3:
                    if (ProjectileLevel == ProjectileLevel.One)
                    {
                        ProjectileLevel = ProjectileLevel.Two;
                        ret = true;
                    }
                    break;
                case 7:
                    if (ProjectileLevel == ProjectileLevel.Two)
                    {
                        ProjectileLevel = ProjectileLevel.Three;
                        ret = true;
                    }
                    break;
                case 11:
                    if (ProjectileLevel == ProjectileLevel.Three)
                    {
                        ProjectileLevel = ProjectileLevel.Four;
                        ret = true;
                    }
                    break;
                case 4:
                    if (EngineLevel == EngineLevel.One)
                    {
                        EngineLevel = EngineLevel.Two;
                        ret = true;
                    }
                    break;
                case 8:
                    if (EngineLevel == EngineLevel.Two)
                    {
                        EngineLevel = EngineLevel.Three;
                        ret = true;
                    }
                    break;
                case 12:
                    if (!Rainbow)
                    {
                        Rainbow = true;
                        ret = true;
                    }
                    break;
            }
            return ret;
        }
    }

    enum ShieldEnum { None = 0, One, Two, Three}
    enum WeaponLevel { One = 1,Two,Three, Four}
    enum EngineLevel { One,Two,Three,Four}
    enum ProjectileLevel { One,Two,Three,Four}

    class Shield
    {
        public ShieldEnum Level = ShieldEnum.None;
        public float ShieldTimer { get; set; }
        public bool Active { get; set; }
        public Vector2 Position { get; set; }
        private static Vector2 origin = new Vector2(25, 25);

        public Shield()
        {

        }
        public static bool HitShield(Vector2 pv, Vector2 ov)
        {
            return (Vector2.Distance(ov, pv + origin) <= 25);
        }
        public bool InShield(Vector2 v)
        {
            return (Vector2.Distance(v, Position + origin) <= 25);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(TextureManager.SpriteSheet, Position, TextureManager.GetRectangle("shield"), Color.White, 0f, origin,1f,SpriteEffects.None, 0.65f);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (Active && Level == ShieldEnum.None)
            {
                Active = false;
                ShieldTimer = 0f;
            }
            if (Active)
            {
                ShieldTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (ShieldTimer >= (1000 + (1000 * (int)Level)))
                {
                    Active = false;
                    ShieldTimer = 0;
                }
            }

        }
    }
}
