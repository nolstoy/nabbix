using System;
using System.Threading;
using Nabbix;

namespace SimplestExample
{
    internal class Program
    {
        private class MyCounter
        {
            private long _incrementing;
            internal void Increment()
            {
                Interlocked.Increment(ref _incrementing);
            }

            [NabbixItem("long_example")]
            public long Incrementing => Interlocked.Read(ref _incrementing);
        }

        private static void Main()
        {
            // Create the instance of our counter
            var counters = new MyCounter();
            
            // Start the agent.
            var agent = new NabbixAgent(10052, counters);

            // Increment the counter. Normally done on API or method call.
            counters.Increment();

            // Shutdown
            Console.ReadKey();
            agent.Stop();
        }
    }
}
