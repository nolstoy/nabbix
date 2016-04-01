using System;

namespace Nabbix.Items
{
    public static class BaseTypeHelper
    {
        public static string GetPropertyValue(object propertyValue)
        {
            if (propertyValue is float)
                return GetFloatValue((float) propertyValue);
            if (propertyValue is double)
                return GetDoubleValue((double) propertyValue);
            if (propertyValue is decimal)
                return GetDecimalValue((decimal) propertyValue);
            return propertyValue.ToString();
        }

        public static string GetPropertyValue(ValueType propertyValue)
        {
            if (propertyValue is float)
                return GetFloatValue((float)propertyValue);
            if (propertyValue is double)
                return GetDoubleValue((double)propertyValue);
            if (propertyValue is decimal)
                return GetDecimalValue((decimal)propertyValue);
            return propertyValue.ToString();
        }

        // Not perfect, but it's close to the maximum values of 
        // https://www.zabbix.com/documentation/2.0/manual/config/items/item

        internal const double MinDoubleValue = -999000000000.0D;
        internal const double MaxDoubleValue = 999000000000.0D;

        internal static string GetDoubleValue(double value)
        {
            value = Math.Min(MaxDoubleValue, value);
            value = Math.Max(MinDoubleValue, value);

            return value.ToString("0.0000");
        }

        internal const decimal MinDecimalValue = -999000000000.0m;
        internal const decimal MaxDecimalValue = 999000000000.0m;

        internal static string GetDecimalValue(decimal value)
        {
            value = Math.Min(MaxDecimalValue, value);
            value = Math.Max(MinDecimalValue, value);

            return value.ToString("0.0000");
        }

        internal const float MinFloatValue = -990000000000.0f;
        internal const float MaxFloatValue = 990000000000.0f;

        internal static string GetFloatValue(float value)
        {
            value = Math.Min(MaxFloatValue, value);
            value = Math.Max(MinFloatValue, value);

            return value.ToString("0.0000");
        }
    }
}