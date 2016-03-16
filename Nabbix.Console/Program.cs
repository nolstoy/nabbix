using System;
using System.Threading;

namespace Nabbix.ConsoleApp
{
    public class SimpleCounters
    {
        private long _incrementing;
        [NabbixItemAttribute("incrementing")]
        public long Incrementing => Interlocked.Read(ref _incrementing);
        public void Increment()
        {
            Interlocked.Increment(ref _incrementing);
        }


        private readonly object _floatLockObj = new object();
        private float _float;
        [NabbixItemAttribute("floating")]
        public float Float
        {
            get { lock (_floatLockObj) return _float; }
            set { lock (_floatLockObj) _float = value; }
        }

        public void GetRandomFloat(Random random)
        {
            double mantissa = (random.NextDouble()*2.0) - 1.0;
            double exponent = Math.Pow(2.0, random.Next(-126, 128));

            lock (_floatLockObj) _float = (float) (mantissa*exponent);
        }


    }

    class Program
    {
        public static float GetRandomFloat(Random random)
        {
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            double exponent = Math.Pow(2.0, random.Next(-126, 128));

            return (float)(mantissa * exponent);
        }

        private static bool _stopped;
        static void Main(string[] args)
        {
            SimpleCounters counters = new SimpleCounters();
            INabbixAgent agent = new NabbixAgent(10052, counters);
            
            var rand = new Random();
            var increaseCounters = new Thread(() =>
            {
                while (_stopped == false)
                {
                    counters.Increment();
                    counters.Float = GetRandomFloat(rand);
                    Thread.Sleep(2000);
                }
            }) { IsBackground =  true};
            increaseCounters.Start();

            Console.ReadKey();

            Console.WriteLine("Stopping Counters");
            _stopped = true;
            increaseCounters.Join();

            Console.WriteLine("Stopping Listener");
            agent.Stop();
        }
    }
}
