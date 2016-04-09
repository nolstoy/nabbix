using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using metrics;
using metrics.Core;
using metrics.Stats;

namespace Nabbix.Metrics.Items
{
    public class NabbixPerformanceMetrics
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NabbixPerformanceMetrics));

        private readonly object _lockObj;
        private readonly HistogramMetric _lastMinute;
        private readonly MeterMetric _meter;
        private readonly Dictionary<Guid, Stopwatch> _stopwatches;

        internal double FifteenMinuteRate => _meter.FifteenMinuteRate;
        internal double FiveMinuteRate => _meter.FiveMinuteRate;
        internal double OneMinuteRate => _meter.OneMinuteRate;
        internal double MeanRate => _meter.MeanRate;
        internal long MeterCount => _meter.Count;

        internal double SampleMax => _lastMinute.SampleMax;
        internal double SampleMean => _lastMinute.SampleMean;
        internal double SampleMin => _lastMinute.SampleMin;
        internal double Max => _lastMinute.Max;
        internal double Mean => _lastMinute.Mean;
        internal double Min => _lastMinute.Min;
        internal double Percentile75 => _lastMinute.Percentiles(0.75D)[0];
        internal double Percentile90 => _lastMinute.Percentiles(0.90D)[0];
        internal double Percentile95 => _lastMinute.Percentiles(0.95D)[0];
        internal double Percentile99 => _lastMinute.Percentiles(0.99D)[0];

        internal int CurrentlyExecutingCount { get { lock (_lockObj) { return _stopwatches.Count; } } }
        internal long OldestRequest {
            get
            {
                lock (_lockObj)
                {
                    return _stopwatches.Count == 0 ? 0 
                        : _stopwatches.Values.Select(watch => watch.ElapsedMilliseconds).Max();
                }
            }
        }

        public NabbixPerformanceMetrics()
            : this(new LimitedTimeSample(new TimeSpan(0, 0, 1, 0), new TimeSpan(0, 0, 1, 0)),
                  MeterMetric.New("counter", TimeUnit.Seconds))
        {
            
        }

        public NabbixPerformanceMetrics(ISample sample, MeterMetric meter)
        {
            if (sample == null) throw new ArgumentNullException(nameof(sample));
            if (meter == null) throw new ArgumentNullException(nameof(meter));

            _lockObj = new object();
            _lastMinute = new HistogramMetric(sample);
            _meter = meter;
            _stopwatches = new Dictionary<Guid, Stopwatch>(1000);
        }

        public Guid Start()
        {
            Guid id = Guid.NewGuid();
            var watch = Stopwatch.StartNew();

            lock (_lockObj)
            {
                _stopwatches.Add(id, watch);
            }

            return id;
        }

        public void Stop(Guid id)
        {
            lock (_lockObj)
            {
                Stopwatch watch;
                if (_stopwatches.TryGetValue(id, out watch))
                {
                    watch.Stop();

                    _meter.Mark();
                    _lastMinute.Update(watch.ElapsedMilliseconds);

                    _stopwatches.Remove(id);
                }
                else
                {
                    Log.WarnFormat("Failed to find Stopwatch for id={0}", id);
                }
            }
        }
    }
}