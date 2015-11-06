using System;
using Leap;
using MimeArm.Models;

namespace MimeArm.DataLayer
{
    public class LeapReader : IDisposable
    {
        private static LeapReader _instance;

        private Controller LeapController { get; }

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

        private void RecieveDataFromListener(object sender, FrameEventArgs args)
        {
            
        }

        public void Dispose()
        {
            LeapController.Dispose();
        }
    }
}
