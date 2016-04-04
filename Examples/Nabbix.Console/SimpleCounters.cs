using System.IO;
using System.Threading;
using Nabbix.Items;
using Nabbix.Metrics.Items;

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

        private readonly object _stringLockObj = new object();
        private string _string;

        internal void Increment()
        {
            Interlocked.Increment(ref _incrementing);
        }

        [NabbixItem("long_example")]
        public long Incrementing => Interlocked.Read(ref _incrementing);

        [NabbixItem("float_example")]
        public float FloatExample
        {
            get { lock (_floatLockObj) return _float; }
            set { lock (_floatLockObj) _float = value; }
        }

        [NabbixItem("double_example")]
        public double DoubleExample
        {
            get { lock (_doubleLockObj) return _double; }
            set { lock (_doubleLockObj) _double = value; }
        }

        [NabbixItem("decimal_example")]
        public decimal DecimalExample
        {
            get { lock (_decimalLockObj) return _decimal; }
            set { lock (_decimalLockObj) _decimal = value; }
        }

        [NabbixItem("string_example")]
        public string StringExample
        {
            get { lock (_stringLockObj) return _string; }
            set { lock (_stringLockObj) _string = value; }
        }
    }
}