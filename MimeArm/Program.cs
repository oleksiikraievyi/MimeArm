using System.Threading;
using MimeArm.BusinessLayer;
using MimeArm.Interfaces;
using System;
using System.Configuration;

namespace MimeArm
{
    class Program
    {
        public static void Main()
        {
            foreach (var key in ConfigurationManager.AppSettings.Keys)
                Console.WriteLine(key.ToString());

            Console.WriteLine("MimeArm, version: " );
            Console.Read();

            using (var comController = new ComController())
            {
                using (var comInterface = new ComInterface(comController))
                {
                    new ManualResetEvent(false).WaitOne();
                }
            }
        }
    }
}
