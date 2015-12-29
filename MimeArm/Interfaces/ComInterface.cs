using System;
using MimeArm.BusinessLayer;
using MimeArm.Models;
using System.Runtime.CompilerServices;

namespace MimeArm.Interfaces
{
    public class ComInterface : Interface<LeapData>, IDisposable
    {
        private ComController leapDataController;
        private bool allowedCommunication = false;

        public ComInterface(Controller<LeapData> controller) : base(controller)
        {
            leapDataController = (ComController)controller;
        }

        public override void SendDataToInterface(object sender, LeapData t)
        {
            if (allowedCommunication)
            {
                Console.WriteLine("Send data to COM");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AllowCommunication()
        {
            allowedCommunication = true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DenyCommunication()
        {
            allowedCommunication = false;
        }
    }
}