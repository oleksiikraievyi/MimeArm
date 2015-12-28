using MimeArm.BusinessLayer;
using System;
using System.Diagnostics;

namespace MimeArm.Interfaces
{
    class UnityInterface : Interface<object>
    {
        public UnityInterface(Controller<object> controller) : base(controller) { }

        public override void SendDataToInterface(object sender, object t)
        {
            var process = new ProcessStartInfo(@"C:\Users\Danya\Documents\Visual Studio 2015\Projects\WindowsFormsApplication1\WindowsFormsApplication1\bin\Debug\WindowsFormsApplication.exe");
            Process.Start(process);
        }
    }
}
