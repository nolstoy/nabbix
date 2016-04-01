using System.Diagnostics;

namespace Nabbix.Items
{
    public class PerformanceCounterItemAttribute : NabbixItemAttribute
    {
        public PerformanceCounterItemAttribute(string zabbixItemKey) : base(zabbixItemKey)
        {
        }

        protected override string GetPropertyValue(string key, object propertyValue)
        {
            var perfCounter = PropertyHelper.GetType<PerformanceCounter>(propertyValue);
            if (perfCounter != null)
            {
                return BaseTypeHelper.GetFloatValue(perfCounter.NextValue());
            }
            return Item.NotSupported;
        }
    }
}