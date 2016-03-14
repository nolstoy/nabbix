using System;

namespace Nabbix.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            INabbixAgent agent = NabbixAgent.Start("127.0.0.1", 10052);
            Console.CancelKeyPress += (sender, arguments) =>
            {
                Console.WriteLine("Stopping Listener");
                agent.Stop();
            };
        }
    }
}
