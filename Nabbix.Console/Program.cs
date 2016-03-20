using System;
using System.Threading;

namespace Nabbix.ConsoleApp
{
    internal class Program
    {
        private static volatile bool _stopped;

        // http://stackoverflow.com/questions/3365337/best-way-to-generate-a-random-float-in-c-sharp
        private static float GetRandomFloat(Random random)
        {
            var mantissa = random.NextDouble()*2.0 - 1.0;
            var exponent = Math.Pow(2.0, random.Next(-126, 128));

            return (float) (mantissa*exponent);
        }

        private static void IncrementCounters(SimpleCounters counters)
        {
            var rand = new Random();
            while (_stopped == false)
            {
                counters.Increment();
                counters.Float = GetRandomFloat(rand);
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