using System;
using System.Collections.Generic;
using System.Linq;
using Nabbix.Items;

namespace Nabbix.Metrics.Items
{
    public class NabbixPerformanceMetricsItemAttribute : NabbixItemAttribute
    {
        private readonly string _zabbixItemPrefix;
        private readonly Dictionary<string, Func<NabbixPerformanceMetrics, ValueType>> _itemMapping;

        private static Dictionary<string, Func<NabbixPerformanceMetrics, ValueType>> CreateMapping()
        {
            var mapping = new Dictionary<string, Func<NabbixPerformanceMetrics, ValueType>>(50);
            mapping.Add("fifteen_minute_rate", m => m.FifteenMinuteRate);
            mapping.Add("five_minute_rate", m => m.FiveMinuteRate);
            mapping.Add("one_minute_rate", m => m.OneMinuteRate);
            mapping.Add("mean_rate", m => m.MeanRate);
            mapping.Add("meter_count", m => m.MeterCount);

            mapping.Add("sample_max", m => m.SampleMax);
            mapping.Add("sample_mean", m => m.SampleMean);
            mapping.Add("sample_min", m => m.SampleMin);
            mapping.Add("max", m => m.Max);
            mapping.Add("mean", m => m.Mean);
            mapping.Add("min", m => m.Min);
            mapping.Add("percentile_75", m => m.Percentile75);
            mapping.Add("percentile_90", m => m.Percentile90);
            mapping.Add("percentile_95", m => m.Percentile95);
            mapping.Add("percentile_99", m => m.Percentile99);

            mapping.Add("currently_executing_count", m => m.CurrentlyExecutingCount);
            mapping.Add("oldest_request", m => m.OldestRequest);

            return mapping;
        }

        private static ICollection<string> GetKeys(string zabbixItemPrefix)
        {
            return CreateMapping().Keys.Select(key => GetCompleteKey(zabbixItemPrefix, key)).ToList();
        }

        private static string GetCompleteKey(string prefix, string key)
        {
            return prefix + "_" + key;
        }

        public NabbixPerformanceMetricsItemAttribute(string zabbixItemPrefix)
            : base(GetKeys(zabbixItemPrefix))
        {
            _zabbixItemPrefix = zabbixItemPrefix;
            _itemMapping = CreateMapping();
        }

        protected override string GetPropertyValue(string key, object propertyValue)
        {
            NabbixPerformanceMetrics metrics = PropertyHelper.GetType<NabbixPerformanceMetrics>(propertyValue);

            key = key.Substring(_zabbixItemPrefix.Length + 1);
            Func<NabbixPerformanceMetrics, ValueType> callback;
            if (!_itemMapping.TryGetValue(key, out callback))
                return Item.NotSupported;

            ValueType valueType = callback(metrics);
            return BaseTypeHelper.GetPropertyValue(valueType);
        }
    }
}