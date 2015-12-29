using System.Threading;
using MimeArm.BusinessLayer;
using MimeArm.Interfaces;
using System;
using System.Configuration;
using System.Collections.Specialized;

namespace MimeArm
{
    class Program
    {
        private const string ProgramInfoSectionName = "ProgramInfo";
        private const string ProgramVersionPropertyName = "ProgramVersion";
        public static ManualResetEvent exit = new ManualResetEvent(false);

        public static void Main()
        {
            var programInfoConfig = (ConfigurationManager.GetSection(ProgramInfoSectionName) as NameValueCollection)?[ProgramVersionPropertyName];
            Console.WriteLine("MimeArm, build: " + programInfoConfig + ". Press Enter to proceed");
            Console.Read();

            using (var comController = new ComController())
            {
                using (var comInterface = new ComInterface(comController))
                {
                    comInterface.AllowCommunication();
                    exit.WaitOne();
                    comInterface.DenyCommunication();
                }
            }
        }
    }
}
