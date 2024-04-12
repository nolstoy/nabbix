using System.Diagnostics;
using Xunit;

namespace Nabbix.Tests
{
    public class WindowsPerformanceCountersTests
    {
        [Fact]
        public void ParseCounter_HappyPath_NonNullReturn()
        {
            PerformanceCounter counterA = WindowsPerformanceCounters.ParseCounter(
                @"perf_counter[""\Processor Information(_Total)\% Processor Time""]");
            PerformanceCounter counterB = WindowsPerformanceCounters.ParseCounter(
                @"perf_counter[""\Memory\Available Bytes""]");

            Assert.True(counterA != null);
            Assert.True(counterB != null);
        }
    }
}
