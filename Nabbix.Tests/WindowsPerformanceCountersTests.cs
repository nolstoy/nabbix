#if NET45
using Xunit;

namespace Nabbix.Tests
{
    public class WindowsPerformanceCountersTests
    {
        [Fact]
        public void MethodName_StateUnderTest_ExpectedBehavior()
        {
            var counterA = WindowsPerformanceCounters.ParseCounter(@"perf_counter[""\Processor Information(_Total)\% Processor Time""]");
            var counterB = WindowsPerformanceCounters.ParseCounter(@"perf_counter[""\Memory\Available Bytes""]");

            Assert.IsNotNull(counterA);
            Assert.IsNotNull(counterB);
        }

    }
}
#endif