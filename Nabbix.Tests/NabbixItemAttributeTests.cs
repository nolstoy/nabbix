using Nabbix.Attributes;
using NUnit.Framework;

namespace Nabbix.Tests
{
    [TestFixture]
    public class NabbixItemAttributeTests
    {
        [Test]
        public void GetFloatValue_SizeExceedsMax_ReturnsMaxAllowedValue()
        {
            const string maxZabbixMySqlValue    = "990000000000.0000";
            const float exceedsMaxSize          =  999999999999.9999f;

            string result = NabbixItemAttribute.GetFloatValue(exceedsMaxSize);
            Assert.AreEqual(maxZabbixMySqlValue, result);
        }

        [Test]
        public void GetFloatValue_SizeExceedsMin_ReturnsMinAllowedValue()
        {
            const string minZabbixMySqlValue    = "-990000000000.0000";
            const float exceedsMinSize          =  -999999999999.9999f;

            string result = NabbixItemAttribute.GetFloatValue(exceedsMinSize);
            Assert.AreEqual(minZabbixMySqlValue, result);
        }

        [Test]
        public void GetFloatValue_SizeIsAcceptable_ReturnsSize()
        {
            const float validSize = 124.99f;

            string result = NabbixItemAttribute.GetFloatValue(validSize);
            Assert.AreEqual("124.9900", result);
        }

        [Test]
        public void GetFloatValue_TooManyDecimals_DecimalIsTrimmed()
        {
            const float validSize = 124.99543f;

            string result = NabbixItemAttribute.GetFloatValue(validSize);
            Assert.AreEqual("124.9954", result);
        }

        [Test]
        public void GetDoubleValue_SizeExceedsMax_ReturnsMaxAllowedValue()
        {
            const string maxZabbixMySqlValue    = "999000000000.0000";
            const double exceedsMaxSize         =  999999999999.9999D;

            string result = NabbixItemAttribute.GetDoubleValue(exceedsMaxSize);
            Assert.AreEqual(maxZabbixMySqlValue, result);
        }

        [Test]
        public void GetDoubleValue_SizeExceedsMin_ReturnsMinAllowedValue()
        {
            const string minZabbixMySqlValue    = "-999000000000.0000";
            const double exceedsMinSize         =  -999999999999.9999D;

            string result = NabbixItemAttribute.GetDoubleValue(exceedsMinSize);
            Assert.AreEqual(minZabbixMySqlValue, result);
        }

        [Test]
        public void GetDoubleValue_SizeIsAcceptable_ReturnsSize()
        {
            const double validSize = 124.99D;

            string result = NabbixItemAttribute.GetDoubleValue(validSize);
            Assert.AreEqual("124.9900", result);
        }

        [Test]
        public void GetDoubleValue_TooManyDecimals_DecimalIsTrimmed()
        {
            const double validSize = 124.99543D;

            string result = NabbixItemAttribute.GetDoubleValue(validSize);
            Assert.AreEqual("124.9954", result);
        }

        [Test]
        public void GetDecimalValue_SizeExceedsMax_ReturnsMaxAllowedValue()
        {
            const string maxZabbixMySqlValue    = "999000000000.0000";
            const decimal exceedsMaxSize        =  999999999999.9999m;

            string result = NabbixItemAttribute.GetDecimalValue(exceedsMaxSize);
            Assert.AreEqual(maxZabbixMySqlValue, result);
        }

        [Test]
        public void GetDecimalValue_SizeExceedsMin_ReturnsMinAllowedValue()
        {
            const string minZabbixMySqlValue    = "-999000000000.0000";
            const decimal exceedsMinSize        =  -999999999999.9999m;

            string result = NabbixItemAttribute.GetDecimalValue(exceedsMinSize);
            Assert.AreEqual(minZabbixMySqlValue, result);
        }

        [Test]
        public void GetDecimalValue_SizeIsAcceptable_ReturnsSize()
        {
            const decimal validSize = 124.99m;

            string result = NabbixItemAttribute.GetDecimalValue(validSize);
            Assert.AreEqual("124.9900", result);
        }

        [Test]
        public void GetDecimalValue_TooManyDecimals_DecimalIsTrimmed()
        {
            const decimal validSize = 124.99543m;

            string result = NabbixItemAttribute.GetDecimalValue(validSize);
            Assert.AreEqual("124.9954", result);
        }


    }
}