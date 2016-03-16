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

        public string GetValue(string key, object instance, PropertyInfo propertyInfo)
        {
            string first = ZabbixItemKeys.First();
            if (key != first)
            {
                throw new ArgumentException($"key is invalid. {key} !- {first}");
            }

            object val = propertyInfo.Get(instance);
            return val?.ToString() ?? Item.NotSupported;
        }
    }
}