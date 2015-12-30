using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MimeArm.BusinessLayer;

namespace MimeArm.Tests
{
    [TestFixture]
    public class ComControllerTests
    {
        [Test]
        public void testTrue_prepareByteArrayToSend()
        {
            var byteArray = ComController.PrepareByteArrayToSend(0, 100, 100, 100, 50, 100, 100, 30, 11);
            Assert.AreEqual(byteArray, new byte[] { 255, 0, 100, 0, 100, 0, 100, 0, 50, 0, 100, 0, 100, 30, 0, 11, 176});
        }

        [Test]
        public void testTrue_prepareByteArrayToSendWithEmptyHeader()
        {
            var byteArray = ComController.PrepareByteArrayToSend(0, 100, 100, 100, 50, 100, 100, 30, 11);
            Assert.AreEqual(byteArray, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 176 });
        }

        [Test]
        public void testTrue_isPackageOk()
        {
            byte[] testValue = { 255, 0, 100, 0, 100, 0, 100, 0, 50, 0, 100, 0, 100, 30, 0, 11, 176 };
            Assert.IsTrue(ComController.IsPackageOk(testValue));
        }

        [Test]
        public void testFalse_isPackageOk()
        {
            byte[] testValue = { 255, 0, 100, 0, 100, 0, 100, 1, 50, 0, 100, 0, 100, 30, 0, 11, 176 };
            Assert.IsFalse(ComController.IsPackageOk(testValue));
        }

        [Test]
        public void test_getExponentialBackoffTime()
        {
            ComController.CurrentBackoffLevel = 0;
            Assert.AreEqual(ComController.GetExponentialBackoffTime(), 0);
        }

    }
}
