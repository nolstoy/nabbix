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
            return GetPropertyValue(propertyValue);
        }

        protected virtual string GetPropertyValue(object propertyValue)
        {
            if (propertyValue is float)
                return BaseTypeHelper.GetFloatValue((float)propertyValue);
            if (propertyValue is double)
                return BaseTypeHelper.GetDoubleValue((double)propertyValue);
            if (propertyValue is decimal)
                return BaseTypeHelper.GetDecimalValue((decimal)propertyValue);
            return propertyValue.ToString();
        }

        internal string GetValue(string key, object instance, PropertyInfo propertyInfo)
        {
            string first = ZabbixItemKeys.First();
            if (key != first)
            {
                throw new ArgumentException($"key is invalid. {key} !- {first}");
            }

            object propertyValue = propertyInfo.Get(instance);
            if (propertyValue == null)
                return Item.NotSupported;

            return GetPropertyValue(key, propertyValue);
        }
    }
}