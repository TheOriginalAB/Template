using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Lidgren.Network;
using DwarfWars.Library;

namespace DwarfWars
{
    class Client
    {
        public NetClient client;
        public ClientPlayer player;
        
        public List<ClientPlayer> allPlayers;

        public Client()
        {
            StartClient();
        }

        public void StartClient()
        {
            var config = new NetPeerConfiguration("hej")
            {
                AutoFlushSendQueue = false
            };
            client = new NetClient(config);
            client.Start();

            string ip = "10.49.250.192";
            int port = 14242;
            client.Connect(ip, port);

            player = new ClientPlayer(100, 100, 0, true);
            allPlayers = new List<ClientPlayer>();
            allPlayers.Add(player);
        }

        public void Movement(string direction)
        {
            var xmovement = direction.Contains("L") ? -5 : direction.Contains("R") ? 5 : 0;
            var ymovement = direction.Contains("U") ? -5 : direction.Contains("D") ? 5 : 0;
            var command = new MovementCommand(player, xmovement, ymovement, direction, ICommand.GenerateRandID());
            
            command.Run();
            SendMessage(command);
        }

        public void ReadMessages()
        {
            NetIncomingMessage message;
            var stop = false;
            while (!stop)
            {
                while ((message = client.ReadMessage()) != null)
                {
                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            {
                                CommandType data = (CommandType)message.ReadByte();
                                string commandId = message.ReadString();
                                byte playerID = 0;

                                ICommand command = null;
                                Player[] cl = new Player[allPlayers.Count];
                                for (int i = 0; i < allPlayers.Count; i++)
                                {
                                    cl[i] = allPlayers[i];
                                }
                                switch (data)
                                {
                                    case CommandType.Movement:
                                        playerID = message.ReadByte();
                                        string direction = message.ReadString();
                                        var xmovement = direction.Contains("L") ? -5 : direction.Contains("R") ? 5 : 0;
                                        var ymovement = direction.Contains("U") ? -5 : direction.Contains("D") ? 5 : 0;
                                        if (playerID == player.ID)
                                        {
                                            xmovement = 0;
                                            ymovement = 0;
                                        }
                                        command = new MovementCommand(Player.GetPlayer(cl, playerID), xmovement, ymovement, direction, commandId);
                                        
                                        break;

                                    case CommandType.Welcome:
                                        playerID = message.ReadByte();
                                        var playerX = message.ReadInt32();
                                        var playerY = message.ReadInt32();
                                        var size = message.ReadByte();
                                        ClientPlayer[] otherPlayers = new ClientPlayer[size];
                                        for(int i = 0; i < size; i++)
                                        {
                                            var tempX = message.ReadInt32();
                                            var tempY = message.ReadInt32();
                                            var tempID = message.ReadByte();
                                            var tempRot = message.ReadFloat();
                                            otherPlayers[i] = new ClientPlayer(tempX, tempY, tempRot, false);
                                            otherPlayers[i].SetID(tempID);
                                            otherPlayers[i].SetPos(tempX, tempY);
                                        }
                                        command = new WelcomeCommand<ClientPlayer>(player, otherPlayers, allPlayers, playerID, playerX, playerY, commandId);
                                        
                                        break;
                                    case CommandType.Connect:
                                        ClientPlayer temp = new ClientPlayer(100, 100, 0, false);
                                        var newPlayerID = message.ReadByte();
                                        temp.SetID(newPlayerID);
                                        command = new ConnectCommand<ClientPlayer>(allPlayers, temp, commandId);
                                        break;

                                    case CommandType.Disconnect:
                                        var leavingPlayerID = message.ReadByte();
                                        command = new DisconnectCommand<ClientPlayer>(allPlayers, (ClientPlayer)Player.GetPlayer(cl, leavingPlayerID), commandId);
                                        break;
                                    case CommandType.Build:
                                        break;
                                    case CommandType.Destroy:
                                        break;
                                }

                                command.Run();


                                break;
                            }
                        case NetIncomingMessageType.DebugMessage:
                            Console.WriteLine(message.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            break;
                        default:
                            break;
                    }
                    client.Recycle(message);
                }
            }
        }

        public void SendMessage(ICommand command)
        {
            NetOutgoingMessage message = CreateMessage(command);
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();


        }

        private NetOutgoingMessage CreateMessage(ICommand command)
        {
            NetOutgoingMessage message = client.CreateMessage();
            message.Write((byte)command.CommandType);
            message.Write(command.ID);
            switch (command.CommandType)
            {
                case CommandType.Movement:
                    var MoveCommand = (MovementCommand)command;
                    message.Write(MoveCommand.Target.ID);
                    message.Write(MoveCommand.MoveString);
                    break;
                case CommandType.Build:
                    break;
                case CommandType.Destroy:
                    break;
            }
            return message;
        }
        
        public void Disconnect()
        {
            client.Disconnect("Bye");
        }
    }
}
