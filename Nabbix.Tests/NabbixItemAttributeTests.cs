using Nabbix.Items;
using NUnit.Framework;

namespace Nabbix.Tests
{
    [TestFixture]
    public class BaseTypeHelperTests
    {
        [Test]
        public void GetFloatValue_SizeExceedsMax_ReturnsMaxAllowedValue()
        {
            const string maxZabbixMySqlValue    = "990000000000.0000";
            const float exceedsMaxSize          =  999999999999.9999f;

            string result = BaseTypeHelper.GetFloatValue(exceedsMaxSize);
            Assert.AreEqual(maxZabbixMySqlValue, result);
        }

        [Test]
        public void GetFloatValue_SizeExceedsMin_ReturnsMinAllowedValue()
        {
            const string minZabbixMySqlValue    = "-990000000000.0000";
            const float exceedsMinSize          =  -999999999999.9999f;

            string result = BaseTypeHelper.GetFloatValue(exceedsMinSize);
            Assert.AreEqual(minZabbixMySqlValue, result);
        }

        [Test]
        public void GetFloatValue_SizeIsAcceptable_ReturnsSize()
        {
            const float validSize = 124.99f;

            string result = BaseTypeHelper.GetFloatValue(validSize);
            Assert.AreEqual("124.9900", result);
        }

        [Test]
        public void GetFloatValue_TooManyDecimals_DecimalIsTrimmed()
        {
            const float validSize = 124.99543f;

            string result = BaseTypeHelper.GetFloatValue(validSize);
            Assert.AreEqual("124.9954", result);
        }

        [Test]
        public void GetDoubleValue_SizeExceedsMax_ReturnsMaxAllowedValue()
        {
            const string maxZabbixMySqlValue    = "999000000000.0000";
            const double exceedsMaxSize         =  999999999999.9999D;

            string result = BaseTypeHelper.GetDoubleValue(exceedsMaxSize);
            Assert.AreEqual(maxZabbixMySqlValue, result);
        }

        [Test]
        public void GetDoubleValue_SizeExceedsMin_ReturnsMinAllowedValue()
        {
            const string minZabbixMySqlValue    = "-999000000000.0000";
            const double exceedsMinSize         =  -999999999999.9999D;

            string result = BaseTypeHelper.GetDoubleValue(exceedsMinSize);
            Assert.AreEqual(minZabbixMySqlValue, result);
        }

        [Test]
        public void GetDoubleValue_SizeIsAcceptable_ReturnsSize()
        {
            const double validSize = 124.99D;

            string result = BaseTypeHelper.GetDoubleValue(validSize);
            Assert.AreEqual("124.9900", result);
        }

        [Test]
        public void GetDoubleValue_TooManyDecimals_DecimalIsTrimmed()
        {
            const double validSize = 124.99543D;

            string result = BaseTypeHelper.GetDoubleValue(validSize);
            Assert.AreEqual("124.9954", result);
        }

        [Test]
        public void GetDecimalValue_SizeExceedsMax_ReturnsMaxAllowedValue()
        {
            const string maxZabbixMySqlValue    = "999000000000.0000";
            const decimal exceedsMaxSize        =  999999999999.9999m;

            string result = BaseTypeHelper.GetDecimalValue(exceedsMaxSize);
            Assert.AreEqual(maxZabbixMySqlValue, result);
        }

        [Test]
        public void GetDecimalValue_SizeExceedsMin_ReturnsMinAllowedValue()
        {
            const string minZabbixMySqlValue    = "-999000000000.0000";
            const decimal exceedsMinSize        =  -999999999999.9999m;

            string result = BaseTypeHelper.GetDecimalValue(exceedsMinSize);
            Assert.AreEqual(minZabbixMySqlValue, result);
        }

        [Test]
        public void GetDecimalValue_SizeIsAcceptable_ReturnsSize()
        {
            const decimal validSize = 124.99m;

            string result = BaseTypeHelper.GetDecimalValue(validSize);
            Assert.AreEqual("124.9900", result);
        }

        [Test]
        public void GetDecimalValue_TooManyDecimals_DecimalIsTrimmed()
        {
            const decimal validSize = 124.99543m;

            string result = BaseTypeHelper.GetDecimalValue(validSize);
            Assert.AreEqual("124.9954", result);
        }
    }
}