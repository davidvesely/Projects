using System.Net.Http.Headers;
using System.Net.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Net.Http.Test.Headers
{
    [TestClass]
    public class ObjectCollectionTest
    {
        [TestMethod]
        public void Ctor_ExecuteBothOverloads_MatchExpectation()
        {
            // Use default validator
            ObjectCollection<string> c = new ObjectCollection<string>();

            c.Add("value1");
            c.Insert(0, "value2");

            ExceptionAssert.Throws<ArgumentNullException>(() => c.Add(null), "Add(null)");
            ExceptionAssert.Throws<ArgumentNullException>(() => c[0] = null, "Insert(0, null)");

            Assert.AreEqual(2, c.Count, "Count");
            Assert.AreEqual("value2", c[0], "c[0]");
            Assert.AreEqual("value1", c[1], "c[1]");

            // Use custom validator
            c = new ObjectCollection<string>(item =>
            {
                if (item == null)
                {
                    throw new InvalidOperationException("custom");
                }
            });

            c.Add("value1");
            c[0] = "value2";

            ExceptionAssert.ThrowsInvalidOperation(() => c.Add(null), "Add(null)");
            ExceptionAssert.ThrowsInvalidOperation(() => c[0] = null, "Insert(0, null)");

            Assert.AreEqual(1, c.Count, "Count");
            Assert.AreEqual("value2", c[0], "c[0]");
        }
    }
}
