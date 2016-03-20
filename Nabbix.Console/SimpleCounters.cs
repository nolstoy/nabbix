using System;
using System.Threading;

namespace Nabbix.ConsoleApp
{
    public class SimpleCounters
    {
        private readonly object _floatLockObj = new object();
        private float _float;
        private long _incrementing;

        private readonly object _doubleLockObj = new object();
        private double _double;

        private readonly object _decimalLockObj = new object();
        private decimal _decimal;

        [NabbixItem("long_example")]
        public long Incrementing => Interlocked.Read(ref _incrementing);

        [NabbixItem("float_example")]
        public float Float
        {
            get { lock (_floatLockObj) return _float; }
            set { lock (_floatLockObj) _float = value; }
        }

        [NabbixItem("double_example")]
        public double Double
        {
            get { lock (_doubleLockObj) return _double; }
            set { lock (_doubleLockObj) _double = value; }
        }

        [NabbixItem("decimal_example")]
        public decimal Decimal
        {
            get { lock (_decimalLockObj) return _decimal; }
            set { lock (_doubleLockObj) _decimal = value; }
        }

        public void Increment()
        {
            Interlocked.Increment(ref _incrementing);
        }
    }
}