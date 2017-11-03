using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using DwarfWars.Library;

namespace DwarfWars
{
    class Client
    {
        private NetClient client;

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
        }

        public void Movement(string direction)
        {
            NetOutgoingMessage message = client.CreateMessage();
            
            message.Write((byte)CommandType.Movement);
            message.Write(direction);
            
            client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            client.FlushSendQueue();
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
                                var data = message.ReadString();
                                Console.WriteLine(data);

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

        public void Disconnect()
        {
            client.Disconnect("Bye");
        }
    }
}
