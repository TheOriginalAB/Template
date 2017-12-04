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

        Placement = -4, Build = 4,
        Remove = -5, Destroy = 5,
        
        Craft = -6, Create = 6,

        Attack = -7, Hit
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
}
