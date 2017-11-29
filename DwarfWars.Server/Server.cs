using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Net;
using DwarfWars.Library;

namespace DwarfWars.Server
{
    public class Server
    {
        private readonly object Lock = new object();
        private NetServer server;
        private List<ServerPlayer> Clients;
        public List<ServerPlayer> _clients { get { lock (Lock) { return Clients; } } set { lock (Lock) { Clients = value; } } }
        private byte currentID;

        public Server()
        {
            currentID = 0;
        }

        public void StartServer()
        {
            var config = new NetPeerConfiguration("hej") { Port = 14242, LocalAddress = new IPAddress(new byte[] { 127, 0, 0, 1 }) };
            server = new NetServer(config);
            server.Start();

            _clients = new List<ServerPlayer>();
        }

        public void ReadMessages()
        {
            NetIncomingMessage message;
            while (true)
            {
                while ((message = server.ReadMessage()) != null)
                {
                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            {
                                var data = (CommandType)message.ReadByte();
                                var commandId = message.ReadString();
                                byte playerID = message.ReadByte();

                                ICommand command = null;

                                Player[] cl = new Player[_clients.Count];
                                for (int i = 0; i < _clients.Count; i++)
                                {
                                    cl[i] = _clients[i];
                                }

                                switch (data)
                                {
                                    case CommandType.Movement:
                                        string direction = message.ReadString();
                                        var xmovement = direction.Contains("L") ? -1 : direction.Contains("R") ? 1 : 0;
                                        var ymovement = direction.Contains("U") ? -1 : direction.Contains("D") ? 1 : 0;
                                        command = new MovementCommand(GetServerPlayer(message), xmovement, ymovement, direction, commandId);

                                        break;
                                    case CommandType.Build:
                                        break;
                                    case CommandType.Destroy:
                                        break;
                                }

                                command.Run();
                                SendCommandToAll(command, playerID);


                                break;
                            }
                        case NetIncomingMessageType.DebugMessage:
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            if (message.SenderConnection.Status == NetConnectionStatus.Connected)
                            {
                                var newPlayer = new ServerPlayer(message.SenderConnection, 100, 100, 0);
                                newPlayer.SetID(currentID);
                                currentID++;

                                var command = new ConnectCommand<ServerPlayer>(_clients, newPlayer, ICommand.GenerateRandID());
                                var welcomeCommand = new WelcomeCommand<ServerPlayer>(newPlayer, _clients.ToArray(), null, newPlayer.ID, 100, 100, ICommand.GenerateRandID());

                                var welcomeMessage = CreateMessage(welcomeCommand);
                                
                                server.SendMessage(welcomeMessage, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                                SendCommandToAll(command, newPlayer.ID);
                                command.Run();
                            }
                            if (message.SenderConnection.Status == NetConnectionStatus.Disconnected)
                            {
                                _clients.Remove(GetServerPlayer(message));
                            }
                            break;
                        default:
                            break;
                    }
                    server.Recycle(message);
                }
            }
        }

        public void SendCommandToAll(ICommand command, byte playerID)
        {
            NetOutgoingMessage message = CreateMessage(command);

            List<ServerPlayer> temp = new List<ServerPlayer>();
            foreach (ServerPlayer player in _clients)
            {
                if (player.ID != playerID)
                {
                    temp.Add(player);
                }
            }
            SendMessage(command, temp);
        }

        public void SendMessage(ICommand command, List<ServerPlayer> list)
        {
            foreach (ServerPlayer player in _clients)
            {
                NetOutgoingMessage message = CreateMessage(command);

                server.SendMessage(message, player.Client, NetDeliveryMethod.ReliableOrdered);
                server.FlushSendQueue();

            }
        }
        
        private NetOutgoingMessage CreateMessage(ICommand command)
        {
            NetOutgoingMessage message = server.CreateMessage();
            
            message.Write((byte)command.CommandType);
            message.Write(command.ID);
            switch (command.CommandType)
            {
                case CommandType.Movement:
                    var MoveCommand = (MovementCommand)command;
                    message.Write(MoveCommand.Target.ID);
                    message.Write(MoveCommand.MoveString);
                    break;
                case CommandType.Connect:
                    var ConnCommand = (ConnectCommand<ServerPlayer>)command;
                    message.Write(ConnCommand.NewPlayer.ID);
                    message.Write(ConnCommand.NewPlayer.XPos);
                    message.Write(ConnCommand.NewPlayer.YPos);
                    break;
                case CommandType.Build:
                    break;
                case CommandType.Destroy:
                    break;
                case CommandType.Welcome:
                    var WelcomeCommand = (WelcomeCommand<ServerPlayer>)command;
                    message.Write(WelcomeCommand.PlayerID);
                    message.Write(WelcomeCommand.Pos[0]);
                    message.Write(WelcomeCommand.Pos[1]);
                    var size = (byte)WelcomeCommand.OtherPlayers.Count();
                    message.Write(size);
                    for(int i = 0; i < size; i++)
                    {
                        message.Write(WelcomeCommand.OtherPlayers[i].XPos);
                        message.Write(WelcomeCommand.OtherPlayers[i].YPos);
                        message.Write(WelcomeCommand.OtherPlayers[i].ID);
                        message.Write(WelcomeCommand.OtherPlayers[i].Rotation);
                    }
                    break;
            }
            return message;
        }

        private ServerPlayer GetServerPlayer(NetIncomingMessage msg)
        {
            foreach (ServerPlayer p in _clients)
            {
                if (p.Client.RemoteUniqueIdentifier.Equals(msg.SenderConnection.RemoteUniqueIdentifier))
                {
                    return p;
                }
            }
            return null;
        }
        
    }
}