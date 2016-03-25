using System;
using System.IO;

namespace Nabbix.Items
{
    public class NabbixDriveInfo
    {
        private readonly string _drive;

        public NabbixDriveInfo(string drive)
        {
            if (string.IsNullOrWhiteSpace(drive))
                throw new ArgumentException("Argument is null or whitespace", nameof(drive));

            _drive = drive;
        }

        internal DriveInfo GetDriveInfo()
        {
            return new DriveInfo(_drive);
        }
    }
}