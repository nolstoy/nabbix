using Common.Logging;

namespace Nabbix.Items
{
    internal class PropertyHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PropertyHelper));

        internal static T GetType<T>(object propertyValue) where T : class 
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
    }
}