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
        
        protected string GenerateRandID()
        {
            string output = "";
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            for (int i = 0; i < 8; i++) ID += chars[(new Random()).Next(0, chars.Length)];
            return output;
        }
    }

    public class MovementCommand : ICommand
    {
        Player Target;
        int XMovement, YMovement;
        

        public MovementCommand(Player target, int xmovement, int ymovement)
        {
            Target = target;
            XMovement = xmovement;
            YMovement = ymovement;
            CommandType = CommandType.Movement;
            ID = GenerateRandID();
            
            
        }
        
        public override void Run()
        {
            Target.XPos += XMovement;
            Target.YPos += YMovement;
        }
    }

    public class ResponseCommand : ICommand
    {
        public ResponseCommand(string id)
        {
            ID = id;
            CommandType = CommandType.Response;
        }

        public override void Run()
        {
            
        }
    }
}
