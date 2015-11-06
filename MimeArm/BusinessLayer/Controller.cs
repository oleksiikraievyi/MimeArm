using System;
using MimeArm.DataLayer;
using MimeArm.Models;

namespace MimeArm.BusinessLayer
{
    public class Controller<T> : IDisposable
    {
        public LeapReader LeapReader { get; }
        public LeapData CurrentLeapData { get; set; }

        public event EventHandler<T> OnRecievedDataFromReader;

        protected Controller()
        {
            LeapReader = LeapReader.Instance;
        }

        protected virtual T TransferToView()
        {
            return Activator.CreateInstance<T>();
        }

        public void RecieveLeapData(object sender, LeapDataEventArgs args)
        {
            // recieve leap data 
        }

        public void Dispose()
        {
            LeapReader.Dispose();
        }
    }
}