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
                var commandBytes = ComController.PrepareByteArrayToSend(255, 430, 190, 220, 0x005A, 0x0200, 0x0100, 10, 0);
                leapDataController.Port.Write(commandBytes, 0, commandBytes.Length);
                Console.WriteLine(string.Join(", ", commandBytes));
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
    }
}