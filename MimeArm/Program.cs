using System.Threading;
using MimeArm.BusinessLayer;
using MimeArm.Interfaces;

namespace MimeArm
{
    class Program
    {
        public static void Main()
        {
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
