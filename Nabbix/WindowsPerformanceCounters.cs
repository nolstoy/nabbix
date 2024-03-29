﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Nabbix.Items;

namespace Nabbix
{
    public class WindowsPerformanceCounters
    {
        private static readonly Dictionary<string, PerformanceCounter> Counters = new Dictionary<string, PerformanceCounter>();

        public static bool IsCounter(string key)
        {
            return IsWindows() && key.StartsWith("perf_counter");
        }

        public static string GetNextValue(string key)
        {
            if (!IsWindows()) throw new NotSupportedException();

            PerformanceCounter counter;
            if (!Counters.TryGetValue(key, out counter))
            {
                counter = ParseCounter(key);
                Counters.Add(key, counter);
            }

            float value = counter.NextValue();
            return BaseTypeHelper.GetFloatValue(value);
        }

        // https://www.zabbix.com/documentation/1.8/manual/config/windows_performance_counters
        public static PerformanceCounter ParseCounter(string key)
        {
            if (!IsWindows()) throw new NotSupportedException();

            int start = key.IndexOf('"');
            int end = key.LastIndexOf('"');
            int middle = key.LastIndexOf('\\');
            if (start == -1 || middle == -1 || end == -1)
            {
                throw new FormatException($"Performance Counter format is invalid {key}");
            }

            int instanceStart = key.IndexOf('(');
            int instanceEnd = key.IndexOf(')');

            int categoryLength = instanceStart == -1
                ? middle - start
                : instanceStart - start;

            const bool readOnly = true;
            string category = key.Substring(start + 2, categoryLength - 2);
            string counterName = key.Substring(middle + 1, end - middle - 1);

            if (instanceStart == -1 || instanceEnd == -1)
            {
                return new PerformanceCounter(category, counterName, readOnly);
            }

            string instance = key.Substring(instanceStart + 1, instanceEnd - instanceStart - 1);
            return new PerformanceCounter(category, counterName, instance, readOnly);
        }

        private static bool IsWindows()
        {
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;

            return pid == PlatformID.Win32S ||
                   pid == PlatformID.Win32Windows ||
                   pid == PlatformID.Win32NT ||
                   pid == PlatformID.WinCE;
        }
    }
}