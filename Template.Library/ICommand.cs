using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Library
{
    public abstract class ICommand
    {
        public abstract void Run();
        public CommandType CommandType;
        public string ID;
        
        public ICommand(CommandType commandType, string id)
        {
            CommandType = commandType;
            ID = id;
        }

        public static string GenerateRandID()
        {
            string output = "";
            Random r = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            for (int i = 0; i < 8; i++)
            {
                output += chars[r.Next(0, chars.Length)];
            }
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

    public class LocationCommand : ICommand
    {
        Player target;
        public LocationCommand(Player target, int xpos, int ypos, string commandid) : base(CommandType.Location, commandid)
        {
            this.target = target;
        }

        public override void Run()
        {
            
        }
    }

    public class ConnectCommand<T> : ICommand where T : Player
    {
        List<T> Players;
        public T NewPlayer;

        public ConnectCommand(List<T> players, T newPlayer, string commandid) : base(CommandType.Connect, commandid)
        {
            Players = players;
            NewPlayer = newPlayer;
        }

        public override void Run()
        {
            Players.Add(NewPlayer);
        }
    }

    public class DisconnectCommand<T> : ICommand where T : Player
    {
        List<T> Players;
        public T LeavingPlayer;

        public DisconnectCommand(List<T> players, T leavingPlayer, string commandid) : base(CommandType.Disconnect, commandid)
        {
            Players = players;
            LeavingPlayer = leavingPlayer;
        }

        public override void Run()
        {
            Players.Remove(LeavingPlayer);
        }
    }

    public class WelcomeCommand<T> : ICommand where T : Player
    {
        public byte PlayerID;
        public int[] Pos;
        public T Player;
        public T[] OtherPlayers;
        public List<T> ClientList;

        public WelcomeCommand(T player, T[] otherplayers, List<T> clientlist, byte playerid, int playerX, int playerY, string commandid) : base(CommandType.Welcome, commandid)
        {
            PlayerID = playerid;
            Player = player;
            OtherPlayers = otherplayers;
            ClientList = clientlist;
            Pos = new int[] { playerX, playerY };
        }

        public override void Run()
        {
            Player.SetID(PlayerID);
            Player.SetPos(Pos[0], Pos[1]);
            ClientList.AddRange(OtherPlayers);
        }
    }
}
