using System;
using System.Linq;

namespace Nabbix.ConsoleApp
{
    public static class RandomGenerator
    {
        private static readonly Random Rand = new Random();

        internal static double NextDouble()
        {
            return Rand.NextDouble();
        }

        // http://stackoverflow.com/questions/3365337/best-way-to-generate-a-random-float-in-c-sharp
        internal static float NextFloat()
        {
            double range = (double)float.MaxValue - (double)float.MinValue;
            double sample = Rand.NextDouble();
            double scaled = (sample * range) + float.MinValue;
            return (float)scaled;
        }

        private static int NextInt32()
        {
            unchecked
            {
                int firstBits = Rand.Next(0, 1 << 4) << 28;
                int lastBits = Rand.Next(0, 1 << 28);
                return firstBits | lastBits;
            }
        }

        // http://stackoverflow.com/questions/609501/generating-a-random-decimal-in-c-sharp
        internal static decimal NextDecimal()
        {
            byte scale = (byte) Rand.Next(29);
            bool sign = Rand.Next(2) == 1;
            return new decimal(NextInt32(),
                NextInt32(),
                NextInt32(),
                sign,
                scale);
        }

        public static string NextString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, Rand.Next(10, 50))
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}