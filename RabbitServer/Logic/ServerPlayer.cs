using System;
using Box2DX.Common;
using RabbitServer.Packets;

namespace RabbitServer.Logic
{
    public class ServerPlayer
    {
        public ushort PlayerSlot;
        public string PlayerName = null;
        public Vec2 Position;
        public /*Room*/ uint CurrentRoom;
        public ushort SpriteIndex { get; set; }
        public ushort ImageIndex { get; set; }
        public float ImageSpeed { get; set; }
        public float ImageXScale { get; set; }
        public ushort Palette { get; set; }
        public bool IsRabbit { get; set; }

        //potential to implement server side object management ;) and holy fuck that would be insane
        //multiplayer alone is absurd, but the idea of co-op is just crazy
        public Server server;
        public Server.ClientInstance client;
        
        public ServerPlayer(Server server, Server.ClientInstance client)
        {
            this.server = server;
            this.client = client;
            this.client.sp = this;
        }

        public void SendPacket(IPacket packet)
        {
            if (!client.client.Connected||!server.WriteQueues.ContainsKey(client.InstanceId)) return;
            server.WriteQueues[client.InstanceId].Add(packet);
        }

        public void Disconnect(string reason)
        {
            SendPacket(new Disconnect(reason));
            Console.WriteLine("Disconnecting {0} from the game",PlayerName ?? $"user with id [{PlayerSlot}]");
        }
    }
}