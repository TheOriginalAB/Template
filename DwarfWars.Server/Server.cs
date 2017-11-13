using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Net;
using DwarfWars.Library;

namespace DwarfWars.Server
{
    public class Server
    {
        private NetServer server;
        private Queue<ICommand> commandQueue;
        public List<ServerPlayer> clients;

        public Server(ServerGame game)
        {
            commandQueue = new Queue<ICommand>();
        }

        public void StartServer()
        {
            var config = new NetPeerConfiguration("DwarfWars") { Port = 14242 };
            server = new NetServer(config);
            server.Start();
            
            clients = new List<ServerPlayer>();
        }

        public void ReadMessages()
        {
            NetIncomingMessage message;

            while ((message = server.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        {
                            var data = (CommandType)message.ReadByte();
                            ICommand command = null;
                            switch (data)
                            {
                                case CommandType.Movement:
                                    string direction = message.ReadString();
                                    var xmovement = direction == "L" ? -1 : direction == "R" ? 1 : 0;
                                    var ymovement = direction == "D" ? -1 : direction == "U" ? 1 : 0;
                                    command = new MovementCommand(GetServerPlayer(message), xmovement, ymovement);

                                    break;
                                case CommandType.Connect:
                                    break;
                                case CommandType.Build:
                                    break;
                                case CommandType.Destroy:
                                    break;
                                case CommandType.Response:
                                    break;
                            }

                            commandQueue.Enqueue(command);

                            new Thread(command.Run).Start();
                            
                            break;
                        }
                    case NetIncomingMessageType.DebugMessage:
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        if (message.SenderConnection.Status == NetConnectionStatus.Connected)
                        {
                            clients.Add( new ServerPlayer(message.SenderConnection.Peer, 100, 100, 0));
                        }
                        if (message.SenderConnection.Status == NetConnectionStatus.Disconnected)
                        {
                            clients.Remove(GetServerPlayer(message));
                        }
                        break;
                    default:
                        break;
                }
                server.Recycle(message);
            }
        }

        public void SendWorldState(ICommand command)
        {
            NetOutgoingMessage message = server.CreateMessage();
            if(commandQueue.Count > 0)
            {
                commandQueue.Dequeue();
                
            }
        }

        private ServerPlayer GetServerPlayer(NetIncomingMessage msg)
        {
            foreach (ServerPlayer p in clients)
            {
                if (p.Client.Equals(msg.SenderConnection.Peer))
                {
                    return p;
                }
            }
            return null;
        }
    }
}
