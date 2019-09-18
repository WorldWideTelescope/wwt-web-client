using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using wwtlib;

namespace wwtlib_tests
{
    [TestClass]
    public class ColorTests
    {
        [TestMethod]
        public void TestFromArgb()
        {
            Color color = Color.FromArgb(1, 2, 3, 4);
            Assert.AreEqual(color.R, 2);
            Assert.AreEqual(color.G, 3);
            Assert.AreEqual(color.B, 4);
            Assert.AreEqual(color.A, 1);
        }
    }
}
