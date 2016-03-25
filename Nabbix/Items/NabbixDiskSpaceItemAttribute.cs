using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Nabbix.Items
{
    public class NabbixDiskSpaceItemAttribute : NabbixItemAttribute
    {
        private static string GetZabbixItem(string prefix, string value)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("Argument is null or whitespace", nameof(prefix));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Argument is null or whitespace", nameof(value));

            return prefix + value;
        }

        public NabbixDiskSpaceItemAttribute(string zabbixItemPrefix)
            : this(GetZabbixItem(zabbixItemPrefix, "available_freespace"),
                GetZabbixItem(zabbixItemPrefix, "total_freespace"),
                GetZabbixItem(zabbixItemPrefix, "total_size"),
                GetZabbixItem(zabbixItemPrefix, "volume_label"))
        {
        }

        public NabbixDiskSpaceItemAttribute(string availableFreeSpaceZabbixItem,
            string totalFreeSpaceZabbixItem,
            string totalSizeZabbixItem,
            string volumeLabelZabbixItem)
            : base(new[] {availableFreeSpaceZabbixItem, totalFreeSpaceZabbixItem, totalSizeZabbixItem, volumeLabelZabbixItem})
        {
            _itemMapping = new Dictionary<string, Func<DriveInfo, string>>();
            _itemMapping.Add(availableFreeSpaceZabbixItem, d => d.AvailableFreeSpace.ToString(CultureInfo.InvariantCulture));
            _itemMapping.Add(totalFreeSpaceZabbixItem, d => d.TotalFreeSpace.ToString(CultureInfo.InvariantCulture));
            _itemMapping.Add(totalSizeZabbixItem, d => d.TotalSize.ToString(CultureInfo.InvariantCulture));
            _itemMapping.Add(volumeLabelZabbixItem, d => d.VolumeLabel);
        }

        private readonly Dictionary<string, Func<DriveInfo, string>> _itemMapping;

        protected override string GetPropertyValue(string key, object propertyValue)
        {
            NabbixDriveInfo driveInfo = PropertyHelper.GetType<NabbixDriveInfo>(propertyValue);
            DriveInfo info = driveInfo.GetDriveInfo();
            if (info == null)
                return Item.NotSupported;

            Func<DriveInfo, string> item;
            if (_itemMapping.TryGetValue(key, out item))
            {
                return item(info);
            }

            return Item.NotSupported;
        }
    }
}