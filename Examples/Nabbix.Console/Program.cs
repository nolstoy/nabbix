using System;
using System.Linq;
using System.Threading;

namespace Nabbix.ConsoleApp
{
    internal class Program
    {
        private static volatile bool _stopped;

        private static void IncrementCounters(RandomGenerator random, int millisecondsTimeout, SimpleCounters counters, AdvancedCounters moreCounters)
        {
            while (_stopped == false)
            {
                counters.Increment();
                counters.FloatExample = random.NextFloat();
                counters.DoubleExample = random.NextDouble();
                counters.DecimalExample = random.NextDecimal();
                counters.StringExample = random.NextString();

                if (millisecondsTimeout == -1)
                {
                    millisecondsTimeout = random.NextInt(1, 10);
                }
                else
                {
                    Thread.Sleep(millisecondsTimeout);
                }
                
            }
        } 

        // ReSharper disable once UseObjectOrCollectionInitializer
        private static Thread IncrementCountersOnBackgroundThread(int millisecondsTimeout, SimpleCounters counters, AdvancedCounters moreCounters)
        {
            var increaseCounters = new Thread(() => IncrementCounters(new RandomGenerator(), millisecondsTimeout, counters, moreCounters));
            increaseCounters.IsBackground = true;
            increaseCounters.Start();

            return increaseCounters;
        }

        private static void Main(string[] args)
        {
            var counters = new SimpleCounters();
            var moreCounters = new AdvancedCounters();
            INabbixAgent agent = new NabbixAgent(10052, counters, moreCounters);

            // 100,000 requests/s for an extended period of time will run out of memory.

            const int numThreads = 16;
            Thread[] threads = new Thread[numThreads];

            for (int i = 0; i < numThreads; i++)
            {
                threads[i] = IncrementCountersOnBackgroundThread(10, counters, moreCounters);
            }            

            Console.ReadKey();
            _stopped = true;
            foreach (var thread in threads)
            {
                thread.Join();
            }

            agent.Stop();
        }
    }
}