using System;
using System.Collections.Generic;
using System.Text;

namespace DwarfWars.Library
{
    public enum CommandType : byte 
    {
        Connect, Movement, Destroy, Build, 
    }

    public class Player
    {
        public int XPos, YPos;


        public Player(int x, int y)
        {
            XPos = x;
            YPos = y;
        }
    }
}
