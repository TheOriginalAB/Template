using System;
using System.Collections.Generic;
using System.Text;

namespace DwarfWars.Library
{
    public enum CommandType : byte 
    {
        Welcome, Connect, Response, Movement, Destroy, Build
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

    public class ThreadCloseToken
    {
        private readonly object Lock = new object();

        private bool _IsRunning;
        public bool IsRunning { get { lock (Lock) { return _IsRunning; } } set { lock (Lock) { _IsRunning = value; } } }

        public ThreadCloseToken()
        {
            IsRunning = true;
        }
    }
}
