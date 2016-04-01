using System;
using System.Globalization;

namespace Nabbix.Items
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NabbixFileCountItemAttribute : NabbixItemAttribute
    {
        public NabbixFileCountItemAttribute(string zabbixItemKey) : base(zabbixItemKey)
        {
        }

        protected override string GetPropertyValue(string key, object propertyValue)
        {
            var fileCount = PropertyHelper.GetType<NabbixFileCount>(propertyValue);
            return fileCount?.GetFileCount().ToString(CultureInfo.InvariantCulture) ?? Item.NotSupported;
        }
    }
}