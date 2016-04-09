using System;
using System.Linq;

namespace Nabbix.ConsoleApp
{
    public class RandomGenerator
    {
        private readonly Random _rand = new Random();

        public int NextInt(int min, int max)
        {
            return _rand.Next(min, max);
        }

        internal long NextLong()
        {
            return _rand.Next(1000, 10000);
        }

        internal double NextDouble()
        {
            return _rand.NextDouble();
        }

        // http://stackoverflow.com/questions/3365337/best-way-to-generate-a-random-float-in-c-sharp
        internal float NextFloat()
        {
            double range = (double)float.MaxValue - (double)float.MinValue;
            double sample = _rand.NextDouble();
            double scaled = (sample * range) + float.MinValue;
            return (float)scaled;
        }

        private int NextInt32()
        {
            unchecked
            {
                int firstBits = _rand.Next(0, 1 << 4) << 28;
                int lastBits = _rand.Next(0, 1 << 28);
                return firstBits | lastBits;
            }
        }

        // http://stackoverflow.com/questions/609501/generating-a-random-decimal-in-c-sharp
        internal decimal NextDecimal()
        {
            byte scale = (byte) _rand.Next(29);
            bool sign = _rand.Next(2) == 1;
            return new decimal(NextInt32(),
                NextInt32(),
                NextInt32(),
                sign,
                scale);
        }

        public string NextString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, _rand.Next(10, 50))
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}