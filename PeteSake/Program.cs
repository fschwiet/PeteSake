using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeteSake
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var commands = ManyConsole.ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
                return ManyConsole.ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -1;
            }
        }
    }
}
