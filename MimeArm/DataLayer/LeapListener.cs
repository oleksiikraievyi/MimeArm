using System;
using Leap;
using MimeArm.Models;

namespace MimeArm.DataLayer
{
    public class LeapListener : Listener
    {
        public event EventHandler<FrameEventArgs> OnRecievedLeapData;

        public LeapListener(EventHandler<FrameEventArgs> leapReaderHandler)
        {
            OnRecievedLeapData += leapReaderHandler;
        }

        public override void OnConnect(Controller controller)
        {
            Console.WriteLine("Leap Motion connected");
        }

        public override void OnDisconnect(Controller controller)
        {
            Console.WriteLine("Leap Motion disconnected");
        }

        public override void OnFrame(Controller controller)
        {
            var frame = controller.Frame();
            if (frame.Hands.IsEmpty) return;

            OnRecievedLeapData?.Invoke(this, new FrameEventArgs(frame));
        }
    }
}