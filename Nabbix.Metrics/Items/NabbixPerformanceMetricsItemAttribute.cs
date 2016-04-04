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

            mapping.Add("large_sample_max", m => m.LargeSampleMax);
            mapping.Add("large_sample_mean", m => m.LargeSampleMean);
            mapping.Add("large_sample_min", m => m.LargeSampleMin);
            mapping.Add("large_max", m => m.LargeMax);
            mapping.Add("large_mean", m => m.LargeMean);
            mapping.Add("large_min", m => m.LargeMin);
            mapping.Add("large_75", m => m.Large75);
            mapping.Add("large_90", m => m.Large90);
            mapping.Add("large_95", m => m.Large95);
            mapping.Add("large_99", m => m.Large99);

            mapping.Add("lastminute_sample_max", m => m.LastMinuteSampleMax);
            mapping.Add("lastminute_sample_mean", m => m.LastMinuteSampleMean);
            mapping.Add("lastminute_sample_min", m => m.LastMinuteSampleMin);
            mapping.Add("lastminute_max", m => m.LastMinuteMax);
            mapping.Add("lastminute_mean", m => m.LastMinuteMean);
            mapping.Add("lastminute_min", m => m.LastMinuteMin);
            mapping.Add("lastminute_75", m => m.LastMinute75);
            mapping.Add("lastminute_90", m => m.LastMinute90);
            mapping.Add("lastminute_95", m => m.LastMinute95);
            mapping.Add("lastminute_99", m => m.LastMinute99);

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

            string completeKey = GetCompleteKey(_zabbixItemPrefix, key);

            Func<NabbixPerformanceMetrics, ValueType> callback;
            if (!_itemMapping.TryGetValue(completeKey, out callback)) return Item.NotSupported;

            ValueType valueType = callback(metrics);
            return BaseTypeHelper.GetPropertyValue(valueType);
        }
    }
}