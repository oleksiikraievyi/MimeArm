using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeArm.BusinessLayer;

namespace MimeArm.BusinessLayer
{
    class UnityController : Controller<object>
    {
        protected override object TransferToView()
        {
            return new object();
        }
    }
}
