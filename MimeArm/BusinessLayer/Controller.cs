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
            LeapReader.OnRecievedDataFromListener += RecieveLeapData;
        }

        protected virtual T TransferToView()
        {
            return Activator.CreateInstance<T>();
        }

        public void RecieveLeapData(object sender, LeapDataEventArgs args)
        {
            CurrentLeapData = args.CurrentLeapData;
            OnRecievedDataFromReader?.Invoke(this, TransferToView());
        }

        public void Dispose()
        {
            LeapReader.Dispose();
        }
    }
}