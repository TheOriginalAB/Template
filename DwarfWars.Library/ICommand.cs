using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfWars.Library
{
    public abstract class ICommand
    {
        public abstract void Run();
        public CommandType CommandType;
        public string ID;
        
        public ICommand(CommandType commandType, string id)
        {

        }

        public static string GenerateRandID()
        {
            string output = "";
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            for (int i = 0; i < 8; i++) output += chars[(new Random()).Next(0, chars.Length)];
            return output;
        }
    }

    public class MovementCommand : ICommand
    {
        public Player Target;
        int XMovement, YMovement;
        public string MoveString;

        public MovementCommand(Player target, int xmovement, int ymovement, string moveChar, string commandid) : base(CommandType.Movement, commandid)
        {
            Target = target;
            XMovement = xmovement;
            YMovement = ymovement;
            MoveString = moveChar;
            
        }
        
        public override void Run()
        {
            Target.XPos += XMovement;
            Target.YPos += YMovement;
        }
    }
    
    public class ConnectCommand<T> : ICommand where T : Player
    {
        List<T> Players;
        public T NewPlayer;

        public ConnectCommand(List<T> players, T newPlayer, string commandid) : base(CommandType.Connect, commandid)
        {
            CommandType = CommandType.Connect;
            Players = players;
            NewPlayer = newPlayer;
        }

        public override void Run()
        {
            Players.Add(NewPlayer);
        }
    }

    public class WelcomeCommand<T> : ICommand where T : Player
    {
        public byte PlayerID;
        public T Player;
        public T[] OtherPlayers;
        public List<T> ClientList;

        public WelcomeCommand(T player, T[] otherplayers, List<T> clientlist, byte playerid, string commandid) : base(CommandType.Welcome, commandid)
        {
            PlayerID = playerid;
            Player = player;
            OtherPlayers = otherplayers;
            ClientList = clientlist;
        }

        public override void Run()
        {
            Player.SetID(PlayerID);
            ClientList.AddRange(OtherPlayers);
        }
    }
}
