using System;
using System.Threading;

namespace Nabbix.ConsoleApp
{
    internal class Program
    {
        private static volatile bool _stopped;

        private static void IncrementCounters(SimpleCounters counters)
        {
            while (_stopped == false)
            {
                counters.Increment();
                counters.FloatExample = RandomGenerator.NextFloat();
                counters.DoubleExample = RandomGenerator.NextDouble();
                counters.DecimalExample = RandomGenerator.NextDecimal();
                counters.StringExample = RandomGenerator.NextString();
                Thread.Sleep(2000);
            }
        }

        // ReSharper disable once UseObjectOrCollectionInitializer
        private static Thread IncrementCountersOnBackgroundThread(SimpleCounters counters)
        {
            var increaseCounters = new Thread(() => IncrementCounters(counters));
            increaseCounters.IsBackground = true;
            increaseCounters.Start();

            return increaseCounters;
        }

        private static void Main()
        {
            var counters = new SimpleCounters();
            INabbixAgent agent = new NabbixAgent(10052, counters);

            var thread = IncrementCountersOnBackgroundThread(counters);
            Console.ReadKey();
            Console.WriteLine("Stopping Counters");

            _stopped = true;
            thread.Join();

            Console.WriteLine("Stopping Listener");
            agent.Stop();
        }
    }
}