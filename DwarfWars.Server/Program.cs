using System;
using System.Threading;
namespace DwarfWars.Server
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Server server = new Server();
            server.StartServer();
            Thread thread = new Thread(server.ReadMessages);
            thread.Start();
            using (var game = new ServerGame(server))
                game.Run();
            thread.Abort();
        }
    }
#endif
}
