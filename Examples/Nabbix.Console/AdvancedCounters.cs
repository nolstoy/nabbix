using Nabbix.Items;

namespace Nabbix.ConsoleApp
{
    public class AdvancedCounters
    {
        [NabbixDiskSpaceItem("c_available_free", "c_total_free", "c_total_size", "c_volume_label")]
        public NabbixDiskSpace DiskSpace { get; } = new NabbixDiskSpace(@"C");

        [NabbixFileCountItem("all_files")]
        public NabbixFileCount NabbixAllFiles { get; } = new NabbixFileCount(@"C:\git\nabbix");

        [NabbixFileCountItem("csharp_files")]
        public NabbixFileCount NabbixFiles { get; } = new NabbixFileCount(@"C:\git\nabbix", "*.cs");
    }
}