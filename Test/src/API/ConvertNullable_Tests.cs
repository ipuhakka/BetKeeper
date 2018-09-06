using NUnit.Framework;
using API;

namespace Test.API
{
    [TestFixture]
    [Category("API")]
    class Conversions_Tests
    {
        [Test]
        public void test_ToNullableBool_return_null()
        {
            Assert.IsNull(Conversions.ToNullableBool("null"));
            Assert.IsNull(Conversions.ToNullableBool(null));
        }

        [Test]
        public void test_ToNullableBool_return_true()
        {
            Assert.IsTrue(Conversions.ToNullableBool("true"));
        }

        [Test]
        public void test_ToNullableBool_return_false()
        {
            Assert.IsFalse(Conversions.ToNullableBool("false"));
        }
    }
}
