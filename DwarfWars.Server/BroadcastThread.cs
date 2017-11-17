using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DwarfWars.Library;

namespace DwarfWars.Server
{
    
    public class BroadcastThread
    {
        public readonly object Lock = new object();
        Thread thread;
        string id;
        List<ServerPlayer> _players;
        List<ServerPlayer> Players { get { lock (Lock) { return _players; } } set { lock (Lock) { _players = value; } } }
        ThreadCloseToken token;


        public BroadcastThread(ThreadStart sendingThread, string id, List<ServerPlayer> recievers, ThreadCloseToken token)
        {
            thread = new Thread(sendingThread);
            this.id = id;
            Players = recievers;
            this.token = token;
        }

        public void HasResponded(byte playerId, string commandId)
        {
            if(commandId == id)
            {
                if(Players.Count > 0)
                {
                    for(int i = 0; i < Players.Count; i++)
                    {
                        if(Players[i].ID == playerId)
                        {
                            Players.Remove(Players[i]);
                            break;
                        }
                    }
                }
                else
                {
                    token.IsRunning = false;
                    thread.Join();
                }
            }
        }
    }
}
