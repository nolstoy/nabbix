using Nabbix.Items;

namespace Nabbix.Metrics.Items
{
    public class NabbixSizeMetricsItemAttribute : NabbixItemAttribute
    {
        public NabbixSizeMetricsItemAttribute(string zabbixItemKey) 
            : base(zabbixItemKey)
        {  
        }

        protected override string GetPropertyValue(string key, object propertyValue)
        {
            var size = PropertyHelper.GetType<NabbixSizeMetrics>(propertyValue);
            if (size == null)
                return Item.NotSupported;

            return BaseTypeHelper.GetPropertyValue(size.GetSizePerSecondForLastMinute());
        }
    }
}