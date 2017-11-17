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
        private NetClient client;
        private ClientPlayer player;
        private List<SendingThread> threads;
        
        public List<ClientPlayer> allPlayers;
        public List<ICommand> recentCommands;

        public Client()
        {
            StartClient();
        }

        public void StartClient()
        {
            var config = new NetPeerConfiguration("DwarfWars")
            {
                AutoFlushSendQueue = false
            };
            client = new NetClient(config);
            client.Start();

            string ip = "10.49.250.192";
            int port = 14242;
            client.Connect(ip, port);

            player = new ClientPlayer(0, 0, 0, true);
            threads = new List<SendingThread>();
            allPlayers = new List<ClientPlayer>();
            allPlayers.Add(player);
            recentCommands = new List<ICommand>();
        }

        public void Movement(string direction)
        {
            var xmovement = direction == "L" ? -1 : direction == "R" ? 1 : 0;
            var ymovement = direction == "D" ? -1 : direction == "U" ? 1 : 0;
            var command = new MovementCommand(player, xmovement, ymovement, direction, ICommand.GenerateRandID());
            
            command.Run();

            ThreadCloseToken token = new ThreadCloseToken();
            SendingThread thread = new SendingThread(() => SendMessage(command, token), command.ID, token);
            threads.Add(thread);
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
                                        var xmovement = direction == "L" ? -1 : direction == "R" ? 1 : 0;
                                        var ymovement = direction == "D" ? -1 : direction == "U" ? 1 : 0;
                                        
                                        command = new MovementCommand(Player.GetPlayer(cl, playerID), xmovement, ymovement, direction, commandId);
                                        
                                        break;

                                    case CommandType.Welcome:
                                        playerID = message.ReadByte();
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
                                        }
                                        command = new WelcomeCommand<ClientPlayer>(player, otherPlayers, allPlayers, playerID, commandId);
                                        
                                        break;
                                    case CommandType.Connect:
                                        ClientPlayer temp = new ClientPlayer(0, 0, 0, false);
                                        command = new ConnectCommand<ClientPlayer>(allPlayers, temp, commandId);

                                        break;
                                    case CommandType.Build:
                                        break;
                                    case CommandType.Destroy:
                                        break;
                                    case CommandType.Response:
                                        foreach (SendingThread st in threads)
                                        {
                                            st.HasResponded(commandId);
                                        }
                                        break;
                                }

                                if (command.CommandType != CommandType.Response && !recentCommands.Contains(command))
                                {
                                    command.Run();
                                    SendResponse(command);
                                    AddToRecent(command);
                                }

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

        public void SendMessage(ICommand command, ThreadCloseToken token)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int lastRunInterval = -1;
            while (token.IsRunning)
            {
                int currentSecond = stopwatch.Elapsed.Seconds;
                if (lastRunInterval != currentSecond && currentSecond % 5 == 0)
                {
                    NetOutgoingMessage message = CreateMessage(command);
                    lastRunInterval = currentSecond;
                    client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
                    client.FlushSendQueue();
                }
            }
            
        }

        private NetOutgoingMessage CreateMessage(ICommand command)
        {
            NetOutgoingMessage message = client.CreateMessage();
            message.Write((byte)command.CommandType);
            switch (command.CommandType)
            {
                case CommandType.Movement:
                    var MoveCommand = (MovementCommand)command;
                    message.Write(MoveCommand.Target.ID);
                    message.Write(MoveCommand.MoveString);
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
            return message;
        }

        public void SendResponse(ICommand command)
        {
            NetOutgoingMessage message = client.CreateMessage();
            message.Write((byte)CommandType.Response);
            message.Write(command.ID);
            message.Write(player.ID);

            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();

        }


        private void AddToRecent(ICommand command)
        {
            while (recentCommands.Count >= 10)
            {
                recentCommands.Remove(recentCommands[0]);
            }
            recentCommands.Add(command);
        }

        public void Disconnect()
        {
            client.Disconnect("Bye");
        }
    }
}
