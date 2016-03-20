using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace Nabbix
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NabbixItemAttribute : Attribute
    {
        public NabbixItemAttribute(string zabbixItemKey)
        {
            if (string.IsNullOrWhiteSpace(zabbixItemKey)) throw new ArgumentNullException(nameof(zabbixItemKey));

            ZabbixItemKeys = new List<string> { zabbixItemKey};
        }

        public List<string> ZabbixItemKeys { get; }

        // Not perfect, but it's close to the maximum values of 
        // https://www.zabbix.com/documentation/2.0/manual/config/items/item

        internal const double MinDoubleValue = -999000000000.0D;
        internal const double MaxDoubleValue =  999000000000.0D;

        internal static string GetDoubleValue(double value)
        {
            value = Math.Min(MaxDoubleValue, value);
            value = Math.Max(MinDoubleValue, value);

            return value.ToString("0.0000");
        }

        internal const decimal MinDecimalValue = -999000000000.0m;
        internal const decimal MaxDecimalValue =  999000000000.0m;

        internal static string GetDecimalValue(decimal value)
        {
            value = Math.Min(MaxDecimalValue, value);
            value = Math.Max(MinDecimalValue, value);

            return value.ToString("0.0000");
        }

        internal const float MinFloatValue = -990000000000.0f;
        internal const float MaxFloatValue =  990000000000.0f;

        internal static string GetFloatValue(float value)
        {
            value = Math.Min(MaxFloatValue, value);
            value = Math.Max(MinFloatValue, value);

            return value.ToString("0.0000");
        }

        internal string GetValue(string key, object instance, PropertyInfo propertyInfo)
        {
            string first = ZabbixItemKeys.First();
            if (key != first)
            {
                throw new ArgumentException($"key is invalid. {key} !- {first}");
            }

            object val = propertyInfo.Get(instance);
            if (val == null)
                return Item.NotSupported;
            if (val is float)
                return GetFloatValue((float)val);
            if (val is double)
                return GetDoubleValue((double)val);
            if (val is decimal)
                return GetDecimalValue((decimal)val);
            return val.ToString();
        }
    }
}