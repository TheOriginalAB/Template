using System;
using System.Collections.Generic;
using System.Text;

namespace DwarfWars.Library
{
    public enum CommandType : sbyte
    {
        Welcome = -1, Connect = 1,
        Goodbye = -2, Disconnect = 2,

        Movement = -3, Location = 3,
        Interact = -4
    }

    public enum TileType : byte
    {
        Air = 0,
        Stone = 1,
        Barrier = 2,
        Dirt = 3,

        CoalOre = 4,
        TinOre = 5,
        CopperOre = 6,
        IronOre = 7,
        GoldOre = 8,
        DiamondOre = 9,

        WorkBench = 10,
        Refinery = 11

    }

    public class Player
    {
        private readonly object Lock = new object();
        private int _XPos;
        public int XPos { get { lock (Lock) { return _XPos; } } set { lock (Lock) { _XPos = value; } } }
        
        private int _YPos;
        public int YPos { get { lock (Lock) { return _YPos; } } set { lock (Lock) { _YPos = value; } } }

        public float Rotation;
        public byte ID;
        
        public Player(int x, int y, float rotation)
        {
            XPos = x;
            YPos = y;
            Rotation = rotation;
        }

        public void SetID(byte id)
        {
            ID = id;
        }

        public void SetPos(int X, int Y)
        {
            XPos = X;
            YPos = Y;
        }

        public static Player GetPlayer(Player[] players, byte ID)
        {
            Player output = null;
            foreach(Player player in players)
            {
                if(player.ID == ID)
                {
                    output = player;
                    break;
                }
            }
            return output;
        }
    }

    public class World
    {

    }
}
