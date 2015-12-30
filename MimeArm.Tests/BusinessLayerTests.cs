using System;
using Leap;
using NUnit.Framework;
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

    [TestFixture]
    public class BusinessLayerTests
    {
        [Test]
        public void ComControllerTest_IsInputAndOutputEqual()
        {
            var leapReader = LeapReader.Instance;
            var testListener = new TestListener(((sender, args) => leapReader.RecieveDataFromListener(sender, args)));
            leapReader.LeapController = new Controller(testListener);

            using (var comController = new ComController())
            {
                var testFrame = new Frame();

                testListener.RaiseEvent(this, testFrame);
                Assert.IsTrue(testFrame == comController.CurrentLeapData.CurrentFrame);
            }
        }
    }
}
