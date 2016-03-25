using System;
using System.IO;

namespace Nabbix.Items
{
    public class NabbixFileCount
    {
        private readonly string _folder;
        private readonly string _searchPattern;
        private readonly SearchOption _searchOption;

        public NabbixFileCount(string folder, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (string.IsNullOrWhiteSpace(folder))
                throw new ArgumentException("Argument is null or whitespace", nameof(folder));

            _folder = folder;
            _searchPattern = searchPattern;
            _searchOption = searchOption;
        }

        internal int GetFileCount() => Directory.GetFiles(_folder, _searchPattern, _searchOption).Length;
    }
}