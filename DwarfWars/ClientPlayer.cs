using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DwarfWars.Library;

namespace DwarfWars
{
    class ClientPlayer : Player
    {
        public bool IsClient;
        public ClientPlayer(int x, int y, float rotation, bool client) : base(x, y, rotation)
        {
            IsClient = client;
        }
    }
}
