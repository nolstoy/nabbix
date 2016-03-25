using System;
using System.Reflection;
using Nabbix.Items;

namespace Nabbix
{
    public class Item
    {
        public const string NotSupported = "ZBX_NOTSUPPORTED";

        private readonly PropertyInfo _property;
        private readonly NabbixItemAttribute _attribute;
        private readonly object _instance;

        public Item(PropertyInfo property, NabbixItemAttribute attribute, object instance)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            _property = property;
            _attribute = attribute;
            _instance = instance;
        }

        public string GetValue(string key)
        {
            return _attribute.GetValue(key, _instance, _property);
        }
    }
}