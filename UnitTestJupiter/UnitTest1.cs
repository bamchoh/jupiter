using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jupiter;
using Opc.Ua;

namespace UnitTestJupiter
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestBooleanVariableInfo()
        {
            var vi = new BooleanVariableInfo();
            Assert.AreEqual(vi.Value, false);
            vi.Value = true;
            Assert.AreEqual(vi.Value, true);
            vi.Value = false;
            Assert.AreEqual(vi.Value, false);
        }

        [TestMethod]
        public void TestSByteeanVariableInfo()
        {
            var vi = new SByteVariableInfo();
            Assert.AreEqual(vi.Value, 0);
            vi.WriteValue = -128;

            Assert.AreEqual(vi.WriteValue, -128);
            Assert.AreEqual(vi.Value, -128);
        }
    }
}
