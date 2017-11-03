using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfWars.Library
{
    public interface ICommand
    {
        void Run();
        CommandType GetCommandType();
        
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
        }

        public CommandType GetCommandType()
        {
            return CommandType.Movement;
        }

        public void Run()
        {
            Target.XPos += XMovement;
            Target.YPos += YMovement;
        }
    }
}
