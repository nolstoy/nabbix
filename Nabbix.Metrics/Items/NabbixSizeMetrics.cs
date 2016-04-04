using System;
using System.Threading;
using metrics.Core;
using metrics.Stats;

namespace Nabbix.Metrics.Items
{
    public class NabbixSizeMetrics
    {
        private readonly HistogramMetric _histogram;
        private long _sizePerSecond;

        public NabbixSizeMetrics()
        {
            _histogram = new HistogramMetric(new LimitedTimeSample(new TimeSpan(0, 0, 1, 0), new TimeSpan(0, 0, 1, 0)));
            _sizePerSecond = 0L;

            PeriodicTaskFactory.Start(() =>
            {
                long lastSecond = Interlocked.Exchange(ref _sizePerSecond, 0);
                _histogram.Update(lastSecond);
            }, 1000); 
        }

        public void Add(long size)
        {
            Interlocked.Add(ref _sizePerSecond, size);
        }

        public double GetSizePerSecondForLastMinute()
        {
            return _histogram.Mean;
        }
    }
}