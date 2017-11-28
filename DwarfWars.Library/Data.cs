using System;
using System.Collections.Generic;
using System.Text;

namespace DwarfWars.Library
{
    public enum CommandType : byte
    {
        Welcome, Movement, Connect, Destroy, Build
    }

    public class Player
    {
        public int XPos;
        
        public int YPos;

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
