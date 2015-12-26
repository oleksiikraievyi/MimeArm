using System;
using Leap;
using MimeArm.Models;

namespace MimeArm.DataLayer
{
    public class LeapReader : IDisposable
    {
        private static LeapReader _instance;

        public Controller LeapController { get; set; }

        public event EventHandler<LeapDataEventArgs> OnRecievedDataFromListener;
        private LeapReader()
        {
            LeapController = new Controller(new LeapListener(RecieveDataFromListener));
        }

        public static LeapReader Instance
        {
            get
            {
                if (ReferenceEquals(_instance, null))
                {
                    _instance = new LeapReader();
                }
                return _instance;
            }
        }

        public void RecieveDataFromListener(object sender, FrameEventArgs args)
        {
            var leapData = new LeapData(args.CurrentFrame);
            OnRecievedDataFromListener?.Invoke(this, new LeapDataEventArgs(leapData));
        }

        public void Dispose()
        {
            LeapController.Dispose();
        }
    }
}
