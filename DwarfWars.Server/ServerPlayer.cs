using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using Template.Library;

namespace Template.Server
{
    public class ServerPlayer : Player
    {
        public NetConnection Client { get; private set; }

        public ServerPlayer(NetConnection client, int x, int y, float rotation) : base(x, y, rotation)
        {
            Client = client;
        }
    }
}
