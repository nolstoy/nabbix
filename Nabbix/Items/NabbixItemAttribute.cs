using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace Nabbix.Items
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NabbixItemAttribute : Attribute
    {
        public NabbixItemAttribute(string zabbixItemKey)
        {
            if (string.IsNullOrWhiteSpace(zabbixItemKey)) throw new ArgumentNullException(nameof(zabbixItemKey));

            ZabbixItemKeys = new List<string> { zabbixItemKey};
        }

        public NabbixItemAttribute(ICollection<string> itemKeys)
        {
            if (itemKeys == null || itemKeys.Count == 0) throw new ArgumentException("Argument is empty collection", nameof(itemKeys));

            ZabbixItemKeys = itemKeys;
        }

        public ICollection<string> ZabbixItemKeys { get; }

        protected virtual string GetPropertyValue(string key, object propertyValue)
        {
            return BaseTypeHelper.GetPropertyValue(propertyValue);
        }

        internal string GetValue(string key, object instance, PropertyInfo propertyInfo)
        {
            if (ZabbixItemKeys.Count == 1)
            {
                string first = ZabbixItemKeys.First();
                if (key != first)
                {
                    throw new ArgumentException($"key is invalid. {key} != {first}");
                }
            }

            object propertyValue = propertyInfo.Get(instance);
            if (propertyValue == null)
                return Item.NotSupported;

            return GetPropertyValue(key, propertyValue);
        }
    }
}