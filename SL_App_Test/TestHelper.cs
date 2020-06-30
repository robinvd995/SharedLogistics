using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SL_App_Test
{
    public class TestHelper
    {
        public static void AssertEqualsi(int expectedValue, int actualValue)
        {
            Assert.IsTrue(expectedValue == actualValue, string.Format("Assertion Failed, expected value: '{0}', actual value {1}", expectedValue, actualValue));
        }

        public static void AssertEqualsb(bool expectedValue, bool actualValue)
        {
            Assert.IsTrue(expectedValue == actualValue, string.Format("Assertion Failed, expected value: '{0}', actual value {1}", expectedValue, actualValue));
        }

        public static void AssertEqualsf(float expectedValue, float actualValue)
        {
            Assert.IsTrue(expectedValue == actualValue, string.Format("Assertion Failed, expected value: '{0}', actual value {1}", expectedValue, actualValue));
        }

        public static void AssertEqualsString(string expectedValue, string actualValue)
        {
            Assert.IsTrue(expectedValue.Equals(actualValue), string.Format("Assertion Failed, expected value: '{0}', actual value {1}", expectedValue, actualValue));
        }
    }
}
