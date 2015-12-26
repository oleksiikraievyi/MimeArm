using System;
using Leap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MimeArm.BusinessLayer;
using MimeArm.DataLayer;
using MimeArm.Models;

namespace MimeArm.Tests
{
    class TestListener : Listener
    {
        public event EventHandler<FrameEventArgs> OnRecievedLeapData;

        public TestListener(EventHandler<FrameEventArgs> handler)
        {
            OnRecievedLeapData += handler;
        }

        public void RaiseEvent(object sender, Frame frame)
        {
            OnRecievedLeapData?.Invoke(this, new FrameEventArgs(frame));
        }
    }

    [TestClass]
    public class BusinessLayerTests
    {
        [TestMethod]
        public void ComControllerTest_IsInputAndOutputEqual()
        {
            var leapReader = LeapReader.Instance;
            var testListener = new TestListener(((sender, args) => leapReader.RecieveDataFromListener(sender, args)));
            leapReader.LeapController = new Controller(testListener);

            using (var comController = new ComController())
            {
                var testFrame = new Frame();
                var testLeapData = new LeapData(testFrame);

                testListener.RaiseEvent(this, testFrame);
                Assert.IsTrue(testLeapData.CurrentFrame == comController.CurrentLeapData.CurrentFrame);
            }
        }
    }
}
