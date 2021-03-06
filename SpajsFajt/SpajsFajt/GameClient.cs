﻿using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Net;
using Microsoft.Xna.Framework.Graphics;

namespace SpajsFajt
{
    class GameClient
    {
        private NetClient netClient;
        private NetIncomingMessage netIn;
        private NetConnection connection;
        public NetConnection Connection
        { get { return connection; } set { connection = value; } }
        public IPEndPoint EndPoint { get; set; }
        public int ID { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public IFocus Focus { get { return (Player)world.GetObject(world.LocalPlayerID); } }

        private World world = new World();
        private Vector2 prevPos;
        private float prevRot;
        private float updateFrequency = 1/30, nextSendUpdate = 1/30;

        public GameClient()
        {
            var npcc = new NetPeerConfiguration("SpajsFajt");
            npcc.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            npcc.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            netClient = new NetClient(npcc);
            netClient.Start();
        }
        
        public void Connect(string host,int port)
        {
            if (netClient.ServerConnection == null)
            {
                netClient.DiscoverKnownPeer(host, port);
                world.Init();
            }
            else
                throw new Exception("Client already connected to server!");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            world.Draw(spriteBatch); 
        }
        internal void Update(GameTime gameTime)
        {
            while ((netIn = netClient.ReadMessage()) != null)
            {
                switch (netIn.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        if (netClient.ConnectionStatus == NetConnectionStatus.Connected)
                        {
                            var netOut = netClient.CreateMessage();
                            netOut.Write((int)GameMessageType.ClientReady);
                            netClient.SendMessage(netOut,NetDeliveryMethod.ReliableOrdered);
                        }
                        else if (netClient.ConnectionStatus == NetConnectionStatus.Disconnected)
                        {
                            Game1.ShouldExit = true;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        switch ((GameMessageType)netIn.ReadInt32())
                        {
                            case GameMessageType.ClientID:
                                ID = netIn.ReadInt32();
                                world.AddObject(new Player(ID));
                                world.LocalPlayerID = ID;
                                world.LocalPlayer = (Player)world.GameObjects[ID];
                                Focus.Position = World.StartPosition;
                                Game1.Focus = Focus;
                                break;
                            case GameMessageType.ClientUpdate:
                                var id = netIn.ReadInt32();
                                var v = new Vector2(netIn.ReadFloat(), netIn.ReadFloat());
                                var r = netIn.ReadFloat();
                                var vel = netIn.ReadFloat();
                                world.DoUpdate(id, v, r, vel);
                                var p = ((Player)(world.GameObjects[id]));
                                p.Boosting = netIn.ReadBoolean();
                                p.Shielding = netIn.ReadBoolean();

                                break;
                            case GameMessageType.ObjectUpdate:
                                id = netIn.ReadInt32();
                                v = new Vector2(netIn.ReadFloat(), netIn.ReadFloat());
                                r = netIn.ReadFloat();
                                vel = netIn.ReadFloat();
                                var type = netIn.ReadInt32();
                                world.DoUpdate(id, v, r, vel, type);
                                break;
                            case GameMessageType.ObjectDeleted:
                                
                                id = netIn.ReadInt32();
                                world.GameObjects.Remove(id);
                                break;
                            case GameMessageType.EnemyDeleted:
                                id = netIn.ReadInt32();
                                ((Enemy)world.GameObjects[id]).Die();
                                Console.WriteLine();
                                break;
                            case GameMessageType.HPUpdate:
                                world.LocalPlayer.Health = netIn.ReadInt32();
                                break;
                            case GameMessageType.PlayerDead:
                                ((Player)world.GameObjects[netIn.ReadInt32()]).Die();
                                break;
                            case GameMessageType.PlayerRespawn:
                                ((Player)world.GameObjects[netIn.ReadInt32()]).Respawn();
                                break;
                            case GameMessageType.PowerUpdate:
                                try
                                {
                                    world.LocalPlayer.PowerLevel = netIn.ReadInt32();
                                    world.LocalPlayer.Boosting = netIn.ReadBoolean();
                                }
                                catch (Exception)
                                {
                                   
                                }
                                break;
                            case GameMessageType.BoostStatus:
                                world.LocalPlayer.Boosting = netIn.ReadBoolean();
                                break;
                            case GameMessageType.CoinPickedUp:
                                ((Gold)world.GameObjects[netIn.ReadInt32()]).Collect = true;
                                break;
                            case GameMessageType.CoinAdded:
                                id = netIn.ReadInt32();
                                var x = netIn.ReadFloat();
                                var y = netIn.ReadFloat();
                                world.GameObjects.Add(id, new Gold(new Vector2(x, y), id));
                                break;
                            case GameMessageType.Rainbow:
                                ((Player)world.GameObjects[netIn.ReadInt32()]).Modifiers.Rainbow = true;
                                break;
                            case GameMessageType.PointsUpdate:
                                world.LocalPlayer.Score = netIn.ReadInt32();
                                break;
                            
                        }
                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        netClient.Connect(netIn.SenderEndPoint);
                        break;
                    
                }
            }
            world.Update(gameTime);
            nextSendUpdate -= gameTime.ElapsedGameTime.Milliseconds;
            if (nextSendUpdate <= 0 && world.LocalPlayerID != -1)
            {
                var p = world.LocalPlayer;
                var netOut = netClient.CreateMessage();
                netOut.Write((int)GameMessageType.ClientPosition);
                netOut.Write(p.ID);
                netOut.Write(p.Position.X);
                netOut.Write(p.Position.Y);
                netOut.Write(p.Rotation);
                netOut.Write(p.Velocity);
                netOut.Write(p.Shielding);
                netClient.SendMessage(netOut, NetDeliveryMethod.Unreliable);
                nextSendUpdate = updateFrequency;

                if (world.RequestedProjectiles > 0)
                {
                    netOut = netClient.CreateMessage();
                    netOut.Write((int)GameMessageType.ProjectileRequest);
                    netOut.Write(p.ID);
                    netOut.Write(world.RequestedProjectiles);
                    netClient.SendMessage(netOut, NetDeliveryMethod.Unreliable);
                    world.RequestedProjectiles = 0; 
                }
                if (p.LastBoostValue != p.BoostRequest)
                {
                    //Boost
                    netOut = netClient.CreateMessage();
                    netOut.Write((int)GameMessageType.BoostRequest);
                    netOut.Write(p.ID);
                    netOut.Write(p.BoostRequest);
                    netClient.SendMessage(netOut, NetDeliveryMethod.ReliableUnordered);
                }
                p.LastBoostValue = p.BoostRequest;

                if (world.UpdateGold)
                {
                    netOut = netClient.CreateMessage();
                    netOut.Write((int)GameMessageType.GoldUpdate);
                    netOut.Write(world.LocalPlayer.ID);
                    netOut.Write(world.LocalPlayer.Gold);
                    netClient.SendMessage(netOut, NetDeliveryMethod.ReliableUnordered);
                }
                foreach (var i in world.Modifications)
                {
                    netOut = netClient.CreateMessage();
                    netOut.Write((int)GameMessageType.ModificationAdded);
                    netOut.Write(world.LocalPlayer.ID);
                    netOut.Write(i);
                    netClient.SendMessage(netOut, NetDeliveryMethod.ReliableUnordered);
                }
                if (world.RemoveShieldPower)
                {
                    netOut = netClient.CreateMessage();
                    netOut.Write((int)GameMessageType.RemoveShieldPower);
                    netOut.Write(world.LocalPlayer.ID);
                    netClient.SendMessage(netOut, NetDeliveryMethod.ReliableUnordered);
                    world.RemoveShieldPower = false;
                }

                world.Modifications.Clear();
            }
            prevPos = Position;
            prevRot = Rotation;
        }

        internal void ShutDown()
        {
            netClient.Shutdown("exiting");
        }
    }
}
