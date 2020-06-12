﻿using System;
using System.Collections.Generic;
 using System.Data;
 using System.Diagnostics;
 using System.IO;
using System.Linq;
 using System.Xml.XPath;
 using RabbitServer.Packets;

namespace RabbitServer.Logic
{
    public class ServerLogic
    {
        public List<ServerPlayer> Players = new List<ServerPlayer>();
        private Server server;
        public ServerLogic(Server server)
        {
            this.server = server;
        }

        private int GetNewPlayerSlot()
        {
            if (Players.Count == 0) return 0;
            for (int i = 0;i<UInt16.MaxValue;i++)
            {
                if (Players.ElementAtOrDefault(i) == null || !Players[i].client.client.Connected)
                {
                    if (Players.ElementAtOrDefault(i) != null)Players.RemoveAt(i);
                    return i;
                }
            }

            return -1;
        }
        
        private ServerPlayer GetPlayer(Server.ClientInstance client)
        {
            return Players.First(x => x.client == client);
        }

        public void ConnectionCrashed(Server.ClientInstance client)
        {
            if (Players.Contains(client.sp)) Players.Remove(client.sp);
        }
        
        public void UserJoined(Server.ClientInstance ci)
        {
            Console.WriteLine("Someone joined!");
            var sp = new ServerPlayer(server,ci);
            if (Players.Count > UInt16.MaxValue) sp.SendPacket(new Disconnect("This server is somehow full."));
            Players.Add(sp);
        }

        public void UserLeft(Server.ClientInstance ci)
        {
            
            try
            {
                //Players.First(s => { return s.client == ci; });
                var sp = GetPlayer(ci);
                Console.WriteLine($"{sp.PlayerName} left. ({sp.PlayerSlot})");
                Broadcast(new PlayerConnect{PlayerId = sp.PlayerSlot, Quitting = false},false,sp, false);
                Players.Remove(sp);
            }
            catch
            {
                Console.WriteLine($"Someone left. ({ci.InstanceId})");
            }
        }

        public void Broadcast(IPacket packet, bool toSelf, ServerPlayer player, bool connected)
        {
            foreach (var p in Players)
            {
                Debug.WriteLine("{0}({1})->{2} (sending = {3})", packet.GetType().Name, player.PlayerSlot, p.PlayerSlot,!toSelf && p != player);
                if (!toSelf && p != player && (!connected || p.PlayerName != null)) p.SendPacket(packet);
            }
        }

        public void PacketReceived(Server.ClientInstance client, IPacket packet)
        {
            //if (!(packet is Movement))Console.WriteLine(packet.ToString());
            ServerPlayer player = GetPlayer(client);
            switch (packet.GetType().Name)
            {
                case "Connect":
                    Console.WriteLine("Someone joined! (Connect Packet)");
                    Connect cn = (Connect) packet;
                    if (cn.Version != Server.Version)
                    {
                        player.Disconnect(
                            $"Your version {cn.Version} is different from the server's version. {Server.Version}");
                        return;
                    }
                    var slot = GetNewPlayerSlot();
                    if (slot == -1)
                    {
                        player.Disconnect("Too many people are online.");
                        return;
                    }
                    cn.PlayerId = player.PlayerSlot = (ushort) slot;
                    player.SendPacket(cn);//kirby right back at ya
                    break;
                case "Disconnect":
                    Disconnect dc = (Disconnect) packet;
                    Console.WriteLine("Disconnected player {0} with reason: \"{1}\"",player.PlayerName != null ? player.PlayerName : "at slot "+ player.PlayerSlot,dc.Reason);
                    //player.client.client.Close();
                    Broadcast(new PlayerConnect{PlayerId = player.PlayerSlot, Quitting = true, Reason = dc.Reason},false,player, true);
                    break;
                case "Movement":
                    Movement mv = (Movement) packet;
                    player.Position = mv.Pos;
                    if (player.CurrentRoom != mv.RoomNumber)Console.WriteLine("{0} went to room number {1}",player.PlayerName,mv.RoomNumber);
                    player.CurrentRoom = mv.RoomNumber;
                    mv.PlayerId = player.PlayerSlot;
                    Debug.WriteLine(mv);
                    Broadcast(mv,false,player, true);
                    break;
                case "PlayerConnect":
                    PlayerConnect pcn = (PlayerConnect) packet;
                    if (!pcn.Quitting)
                    {
                        Console.WriteLine($"{pcn.Name} joined! (Player Connection Packet)");
                        player.PlayerName = pcn.Name;
                        player.Position = pcn.Pos;
                        player.CurrentRoom = pcn.RoomNumber;
                    }
                    pcn.PlayerId = player.PlayerSlot;
                    Broadcast(pcn, false, player, false);
                    foreach (var cpl in Players)
                    {
                        if (cpl != player && cpl.client.client.Connected)
                        {
                            Console.WriteLine("{0}<-{1}",player.PlayerSlot,cpl.PlayerSlot);
                            player.SendPacket(new PlayerConnect
                            {
                                Name = cpl.PlayerName,
                                Quitting = false,
                                Inform = true,
                                Pos = cpl.Position,
                                RoomNumber = cpl.CurrentRoom,
                                PlayerId = cpl.PlayerSlot
                            });
                            player.SendPacket(new SpriteChanged
                            {
                                SpriteIndex = cpl.SpriteIndex,
                                ImageIndex = cpl.ImageIndex,
                                ImageSpeed = cpl.ImageSpeed,
                                ImageXScale = cpl.ImageXScale,
                                Palette = cpl.Palette,
                                PlayerId = cpl.PlayerSlot,
                                IsRabbit = cpl.IsRabbit
                            });
                        }
                    }
                    break;
                case "SpriteChanged":
                    SpriteChanged spc = (SpriteChanged) packet;
                    player.SpriteIndex = spc.SpriteIndex;
                    player.ImageIndex = spc.ImageIndex;
                    player.ImageSpeed = spc.ImageSpeed;
                    player.ImageXScale = spc.ImageXScale;
                    player.Palette = spc.Palette;
                    player.IsRabbit = spc.IsRabbit;
                    spc.PlayerId = player.PlayerSlot;
                    Debug.WriteLine(spc);
                    Broadcast(spc,false,player, true);
                    break;
                case "RerequestFaker":
                    RerequestFaker rrq = new RerequestFaker();
                    if (rrq.PlayerIdOf == player.PlayerSlot) break;
                    Debug.WriteLine(rrq);
                    var faker = Players[rrq.PlayerIdOf];
                    player.SendPacket(new PlayerConnect
                    {
                        PlayerId = rrq.PlayerIdOf,
                        Inform = true,
                        Name = faker.PlayerName,
                        Pos = faker.Position,
                        Quitting = false,
                        RoomNumber = faker.CurrentRoom
                    });
                    player.SendPacket(new SpriteChanged
                    {
                        PlayerId = rrq.PlayerIdOf,
                        Palette = faker.Palette,
                        ImageIndex = faker.ImageIndex,
                        ImageSpeed = faker.ImageSpeed,
                        ImageXScale = faker.ImageXScale,
                        SpriteIndex = faker.SpriteIndex,
                        IsRabbit = faker.IsRabbit
                    });
                    break;
            }
        }
    }
}