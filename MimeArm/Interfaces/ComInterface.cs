using System;
using MimeArm.BusinessLayer;
using MimeArm.Models;

namespace MimeArm.Interfaces
{
    public class ComInterface : Interface<LeapData>
    {
        public ComInterface(Controller<LeapData> controller) : base(controller) { }

        public override void SendDataToInterface(object sender, LeapData t)
        {
            Console.WriteLine("Send data to COM");
        }
    }
}