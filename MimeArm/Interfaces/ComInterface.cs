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
                Console.Write("Writing to COM: ");
                Console.WriteLine(string.Join(", ", leapDataController.SendMoveCommand(430 + t.PalmPosition.x * 160, 190 + t.PalmPosition.y * 20, 220 - t.PalmPosition.z * 40, 10)));
                //Program.exit.Set();
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

        public override void Dispose()
        {
            leapDataController.Dispose();
        }
    }
}