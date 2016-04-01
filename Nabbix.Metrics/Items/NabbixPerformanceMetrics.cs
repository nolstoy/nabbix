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
        private readonly HistogramMetric _largerSample;
        private readonly HistogramMetric _lastMinute;
        private readonly MeterMetric _meter;
        private readonly Dictionary<Guid, Stopwatch> _stopwatches;

        internal double FifteenMinuteRate => _meter.FifteenMinuteRate;
        internal double FiveMinuteRate => _meter.FiveMinuteRate;
        internal double OneMinuteRate => _meter.OneMinuteRate;
        internal double MeanRate => _meter.MeanRate;
        internal long MeterCount => _meter.Count;

        internal double LargeSampleMax => _largerSample.SampleMax;
        internal double LargeSampleMean => _largerSample.SampleMean;
        internal double LargeSampleMin => _largerSample.SampleMin;
        internal double LargeMax => _largerSample.Max;
        internal double LargeMean => _largerSample.Mean;
        internal double LargeMin => _largerSample.Min;
        internal double Large75 => _largerSample.Percentiles(0.75D)[0];
        internal double Large90 => _largerSample.Percentiles(0.90D)[0];
        internal double Large95 => _largerSample.Percentiles(0.95D)[0];
        internal double Large99 => _largerSample.Percentiles(0.99D)[0];

        internal double LastMinuteSampleMax => _lastMinute.SampleMax;
        internal double LastMinuteSampleMean => _lastMinute.SampleMean;
        internal double LastMinuteSampleMin => _lastMinute.SampleMin;
        internal double LastMinuteMax => _lastMinute.Max;
        internal double LastMinuteMean => _lastMinute.Mean;
        internal double LastMinuteMin => _lastMinute.Min;
        internal double LastMinute75 => _lastMinute.Percentiles(0.75D)[0];
        internal double LastMinute90 => _lastMinute.Percentiles(0.90D)[0];
        internal double LastMinute95 => _lastMinute.Percentiles(0.95D)[0];
        internal double LastMinute99 => _lastMinute.Percentiles(0.99D)[0];

        internal int CurrentlyExecutingCount { get { lock (_lockObj) { return _stopwatches.Count; } } }
        internal long OldestRequest { get { lock (_lockObj) { return _stopwatches.Values.Select(watch => watch.ElapsedMilliseconds).Max(); } } }

        public NabbixPerformanceMetrics()
        {
            _lockObj = new object();
            _largerSample = new HistogramMetric(new UniformSample(10000));
            _lastMinute = new HistogramMetric(new LimitedTimeSample(new TimeSpan(0, 0, 1, 0), new TimeSpan(0, 0, 1, 0)));
            _meter = MeterMetric.New("counter", TimeUnit.Milliseconds);
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
                    _largerSample.Update(watch.ElapsedMilliseconds);
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