using System;
using System.Collections.Generic;
using System.Text;

namespace DwarfWars.Library
{
    public enum CommandType : byte 
    {
        Connect, Movement, Destroy, Build, Response
    }

    public class Player
    {
        public int XPos, YPos;
        public float Rotation;


        public Player(int x, int y, float rotation)
        {
            XPos = x;
            YPos = y;
            Rotation = rotation;
        }
    }
}
