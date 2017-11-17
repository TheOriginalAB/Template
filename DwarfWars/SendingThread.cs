using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DwarfWars.Library;

namespace DwarfWars
{
    class SendingThread
    {
        Thread thread;
        string id;
        ThreadCloseToken token;

        public SendingThread(ThreadStart sendingThread, string id, ThreadCloseToken token)
        {
            thread = new Thread(sendingThread);
            this.id = id;
            this.token = token;
            thread.Start();
        }

        public void HasResponded(string commandId)
        {
            if(commandId == id)
            {
                token.IsRunning = false;
                thread.Join();
            }
        }
    }
}
