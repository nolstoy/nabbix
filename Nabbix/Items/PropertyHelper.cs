using System;
using Common.Logging;

namespace Nabbix.Items
{
    public static class PropertyHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PropertyHelper));

        public static T GetType<T>(object propertyValue) where T : class 
        {
            var propertyType = propertyValue as T;
            if (propertyType == null)
            {
                Log.ErrorFormat(
                    "propertyValue is of the wrong type. It must be associated with a {0} property, but it's associated with: {1}",
                    typeof(T).Name,
                    propertyValue.GetType().Name);
            }

            return null;
        }


        internal static string GetZabbixItem(string prefix, string value)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("Argument is null or whitespace", nameof(prefix));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Argument is null or whitespace", nameof(value));

            return prefix + "_" + value;
        }
    }
}